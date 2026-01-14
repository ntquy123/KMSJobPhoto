using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using erpsolution.dal.EF;
using erpsolution.entities;
using erpsolution.service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Renci.SshNet;
using service.Common.Base;

namespace erpsolution.service.KMSJobPhotoMobile
{
    public class AuditResultRegistrationService : ServiceBase<AuditTodoRow>, IAuditResultRegistrationService
    {
        private const string DefaultPhotoName = "imagename.jpg";
        private const string PhotoDeviceMobile = "MOBILE";
        private const string SftpHost = "10.10.1.208";
        private const string SftpUser = "pkuser";
        private const string SftpPassword = "1234@@";
        private const string SftpRootPath = "/home/pkuser/kmsjobphoto";
        private static readonly object TimestampLock = new();
        private static long _lastUnixTimestampMs;
        private readonly string _auditImageRootPath;
        private readonly string _auditImageBaseUrl;

        public AuditResultRegistrationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            var configuration = (IConfiguration?)serviceProvider.GetService(typeof(IConfiguration));
            _auditImageRootPath = configuration?.GetValue<string>("AppSettings:AuditImageRootPath")
                                 ?? throw new InvalidOperationException("Audit image root path is not configured.");
            _auditImageBaseUrl = configuration?.GetValue<string>("AppSettings:AuditImageBaseUrl") ?? string.Empty;
        }

        public override string PrimaryKey => string.Empty;

        public async Task<List<AuditTodoRow>> GetTodoListAsync(AmtTodoRequest request)
        {
            var statusFlag = NormalizeStatus(request.Status);
            string sql = @"SELECT RESMST.AUDPLN_NO
      ,RESMST.CATCODE
      ,RESMST.CORRECTION_NO      
      ,PLNDTL.DTL_STATUS AS STATUS
      ,CASE PLNDTL.DTL_STATUS 
            WHEN 'Y' THEN 'Completed' 
            WHEN 'P' THEN 'To Do' 
            WHEN 'N' THEN 'To Do' 
            END AS STATUS_NAME
      ,TO_CHAR(PLNDTL.DTL_COMPLETION_DATE,'YYYY-MM-DD') AS DTL_COMPLETION_DATE       
      ,PLNDTL.PICID 
      ,PKAMS.SF_USERNAME(PLNDTL.PICID) AS PICNAME
      ,TO_CHAR(PLNDTL.TARGET_DATE,'YYYY-MM-DD') AS TARGET_DATE                   
      ,RESMST.DETAILED_FINDING
      ,RESMST.CORRECTIVE_ACTION
      ,PLNMST.CORPORATION
      ,CORP.CORPORATIONNM_FORMAL AS CORPORATIONNAME     
      ,PLNMST.DEPARTMENT AS DEPT 
      ,DEPT.DEPTNAME      
      ,TO_CHAR(RESMST.CORRECTED_DATE,'YYYY-MM-DD') AS CORRECTED_DATE 
      ,PHOTO.PHOTOCNT
      ,TO_CHAR(PHOTO.UPLOADDATE,'YYYY-MM-DD') AS UPLOAD_DATE 
FROM PKAMT.KMS_AUDPLN_MST PLNMST
LEFT OUTER JOIN PKAMT.KMS_AUDPLN_DTL PLNDTL ON PLNDTL.AUDPLN_NO = PLNMST.AUDPLN_NO
LEFT OUTER JOIN PKAMT.VIEW_AUDIT_ASSESSMENT_ITEM ASSE ON ASSE.ASS_CATCODE = PLNDTL.CATCODE
LEFT OUTER JOIN PKAMT.KMS_AUDRES_MST RESMST ON RESMST.AUDPLN_NO = PLNDTL.AUDPLN_NO AND RESMST.CATCODE = PLNDTL.CATCODE  
LEFT OUTER JOIN PKAMS.V_CORPORATION CORP ON CORP.CORPORATIONCD_FORMAL = PLNMST.CORPORATION
LEFT OUTER JOIN PKAMS.V_HRM_DEPTNAME DEPT ON DEPT.NATION = CORP.NATION AND DEPT.DEPTCODE = PLNMST.DEPARTMENT    
LEFT JOIN LATERAL (
                SELECT COUNT(*) AS PHOTOCNT
                      ,MAX(CRTDATE) AS UPLOADDATE
                FROM PKAMT.KMS_AUDRES_PHO
                WHERE AUDPLN_NO   = RESMST.AUDPLN_NO
                AND CATCODE       = RESMST.CATCODE
                AND CORRECTION_NO = RESMST.CORRECTION_NO
) PHOTO ON 1=1
WHERE 1=1
AND PLNDTL.TARGET_DATE >= TO_DATE(:pFromDate, 'YYYY-MM-DD') AND PLNDTL.TARGET_DATE <= TO_DATE(:pToDate, 'YYYY-MM-DD')
AND RESMST.DETAILED_FINDING IS NOT NULL
AND PLNDTL.PICID = :pPlayerId
AND 1 = CASE :pStatus WHEN 'Y' THEN (CASE WHEN PLNDTL.DTL_STATUS IN ('Y') THEN 1 END)
                   WHEN 'N' THEN (CASE WHEN PLNDTL.DTL_STATUS IN ('N', 'P') THEN 1 END)
                   WHEN 'ALL' THEN 1               
                   END                   
ORDER BY PLNDTL.TARGET_DATE
        ,PLNMST.AUDPLN_NO
        ,PLNDTL.CATCODE
        ,RESMST.CORRECTION_NO";

            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Varchar2, request.FromDate.ToString("yyyy-MM-dd"), ParameterDirection.Input);
            var pToDate = new OracleParameter("pToDate", OracleDbType.Varchar2, request.ToDate.ToString("yyyy-MM-dd"), ParameterDirection.Input);
            var pPlayerId = new OracleParameter("pPlayerId", OracleDbType.Varchar2, request.PlayerId, ParameterDirection.Input);
            var pStatus = new OracleParameter("pStatus", OracleDbType.Varchar2, statusFlag, ParameterDirection.Input);

            var rows = await _amtContext.AuditTodoRows
                .FromSqlRaw(sql, pFromDate, pToDate, pPlayerId, pStatus)
                .ToListAsync();

            return rows;
        }

        public async Task<List<AuditResultPhotoUploadResponse>> UploadPhotoAsync(AuditResultPhotoUploadRequest request, string? baseUrl = null)
        {
            if (request.Photos == null || !request.Photos.Any())
            {
                throw new ArgumentException("At least one photo is required", nameof(request.Photos));
            }

            var folderName = $"{request.AudplnNo}-{request.Catcode}-{request.CorrectionNo}";
            var useSftp = request.Test;
            var folderPath = Path.Combine(_auditImageRootPath, folderName);
            var sftpFolderPath = $"{SftpRootPath.TrimEnd('/')}/{folderName}";
            if (!useSftp)
            {
                Directory.CreateDirectory(folderPath);
            }

            var savedFiles = new List<SavedPhotoInfo>();
            IDbContextTransaction? transaction = null;
            SftpClient? sftpClient = null;
            try
            {
                if (useSftp)
                {
                    sftpClient = CreateSftpClient();
                    sftpClient.Connect();
                    EnsureSftpDirectory(sftpClient, sftpFolderPath);
                    foreach (var photo in request.Photos)
                    {
                        var storedFileName = GenerateUniqueTimestampFileName(photo.FileName);
                        var remoteFilePath = $"{sftpFolderPath}/{storedFileName}";
                        await UploadFileToSftpAsync(sftpClient, photo, remoteFilePath);
                        savedFiles.Add(new SavedPhotoInfo(remoteFilePath, storedFileName, photo, true));
                    }
                }
                else
                {
                    foreach (var photo in request.Photos)
                    {
                        var storedFileName = GenerateUniqueTimestampFileName(photo.FileName);
                        var filePath = Path.Combine(folderPath, storedFileName);
                        await SaveFileAsync(photo, filePath);
                        savedFiles.Add(new SavedPhotoInfo(filePath, storedFileName, photo, false));
                    }
                }

                transaction = await _amtContext.Database.BeginTransactionAsync();
                var photoRootPath = useSftp ? SftpRootPath : _auditImageRootPath;
                var response = await SavePhotoMetadataAsync(request, folderName, savedFiles, baseUrl, photoRootPath);
                await transaction.CommitAsync();
                return response;
            }
            catch
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                if (useSftp)
                {
                    DeleteSftpFiles(savedFiles, sftpClient);
                }
                else
                {
                    foreach (var file in savedFiles)
                    {
                        if (File.Exists(file.FilePath))
                        {
                            File.Delete(file.FilePath);
                        }
                    }
                }

                throw;
            }
            finally
            {
                sftpClient?.Dispose();
                transaction?.Dispose();
            }
        }

        private string NormalizeStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return "ALL";
            }

            switch (status.Trim().ToUpperInvariant())
            {
                case "COMPLETED":
                    return "Y";
                case "TODO":
                case "TO DO":
                    return "N";
                default:
                    return "ALL";
            }
        }

        private async Task<List<AuditResultPhotoUploadResponse>> SavePhotoMetadataAsync(
            AuditResultPhotoUploadRequest request,
            string folderName,
            List<SavedPhotoInfo> savedFiles,
            string? baseUrl,
            string photoRootPath)
        {
            var auditResult = await _amtContext.KmsAudresMsts
                .FirstOrDefaultAsync(x =>
                    x.AudplnNo == request.AudplnNo &&
                    x.Catcode == request.Catcode &&
                    x.CorrectionNo == request.CorrectionNo);

            if (auditResult == null)
            {
                throw new InvalidOperationException("Audit result not found for the provided keys.");
            }

            var currentUser = _currentUser.UserId.ToString();
            var now = DateTime.Now;

            auditResult.CorrectiveAction = request.CorrectiveAction;
            auditResult.CorrectedDate = now;
            auditResult.Uptid = currentUser;
            auditResult.Uptdate = now;
            _amtContext.KmsAudresMsts.Update(auditResult);
            await _amtContext.SaveChangesAsync();
            var nextSeq = await GetNextPhotoSequenceAsync(request);
            var responses = new List<AuditResultPhotoUploadResponse>();
            foreach (var savedFile in savedFiles)
            {
                var photoLink = Path.Combine(photoRootPath, folderName).Replace("\\", "/");
                var photoUrl = BuildPhotoUrl(folderName, savedFile.StoredFileName, baseUrl);
                var photoEntity = new KmsAudresPho
                {
                    AudplnNo = request.AudplnNo,
                    Catcode = request.Catcode,
                    CorrectionNo = request.CorrectionNo,
                    PhoSeq = nextSeq,
                    PhoFile = savedFile.StoredFileName,
                    PhoName = DefaultPhotoName,
                    PhoSize = savedFile.File.Length,
                    PhoLink = photoLink,
                    PhoDesc = request.PhotoDescription,
                    PhoDevice = PhotoDeviceMobile,
                    Crtid = currentUser,
                    Crtdate = now,
                    Uptid = currentUser,
                    Uptdate = now
                };

                _amtContext.KmsAudresPhos.Add(photoEntity);

                responses.Add(new AuditResultPhotoUploadResponse
                {
                    AudplnNo = request.AudplnNo,
                    Catcode = request.Catcode,
                    CorrectionNo = request.CorrectionNo,
                    PhotoSeq = nextSeq,
                    FileName = savedFile.StoredFileName,
                    FileLink = photoUrl,
                    PhotoDescription = request.PhotoDescription,
                    UploadedAt = now
                });

                nextSeq++;
            }

            await _amtContext.SaveChangesAsync();

            return responses;
        }

        public async Task<List<AuditResultPhotoListResponse>> GetPhotoListAsync(AuditResultPhotoListRequest request, string? baseUrl = null)
        {
            var photos = await _amtContext.KmsAudresPhos
                .Where(x => x.AudplnNo == request.AudplnNo &&
                            x.Catcode == request.Catcode &&
                            x.CorrectionNo == request.CorrectionNo)
                .OrderBy(x => x.PhoSeq)
                .ToListAsync();

            var folderName = $"{request.AudplnNo}-{request.Catcode}-{request.CorrectionNo}";
            return photos
                .Where(x => !string.IsNullOrWhiteSpace(x.PhoFile))
                .Select(x => new AuditResultPhotoListResponse
                {
                    AudplnNo = request.AudplnNo,
                    Catcode = request.Catcode,
                    CorrectionNo = request.CorrectionNo,
                    PhotoSeq = x.PhoSeq,
                    FileName = x.PhoFile ?? string.Empty,
                    PhotoUrl = BuildPhotoUrl(folderName, x.PhoFile ?? string.Empty, baseUrl),
                    PhotoDescription = x.PhoDesc,
                    UploadedAt = x.Crtdate
                })
                .ToList();
        }

        private async Task<decimal> GetNextPhotoSequenceAsync(AuditResultPhotoUploadRequest request)
        {
            var currentMax = await _amtContext.KmsAudresPhos
                .Where(x => x.AudplnNo == request.AudplnNo &&
                            x.Catcode == request.Catcode &&
                            x.CorrectionNo == request.CorrectionNo)
                .MaxAsync(x => (decimal?)x.PhoSeq);

            return (currentMax ?? 0) + 1;
        }

        private static async Task SaveFileAsync(IFormFile file, string path)
        {
            await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(stream);
        }

        private string BuildPhotoUrl(string folderName, string fileName, string? baseUrl = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            var resolvedBaseUrl = string.IsNullOrWhiteSpace(baseUrl) ? _auditImageBaseUrl : baseUrl;
            if (string.IsNullOrWhiteSpace(resolvedBaseUrl))
            {
                return Path.Combine(_auditImageRootPath, folderName, fileName).Replace("\\", "/");
            }

            return $"{resolvedBaseUrl.TrimEnd('/')}/{folderName}/{fileName}";
        }

        private static string GenerateUniqueTimestampFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var timestamp = GenerateUniqueUnixTimestampMs();
            return $"FILE_{timestamp}{extension}";
        }

        private static long GenerateUniqueUnixTimestampMs()
        {
            var current = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            lock (TimestampLock)
            {
                if (current <= _lastUnixTimestampMs)
                {
                    current = _lastUnixTimestampMs + 1;
                }

                _lastUnixTimestampMs = current;
                return current;
            }
        }

        private static SftpClient CreateSftpClient()
        {
            return new SftpClient(SftpHost, SftpUser, SftpPassword);
        }

        private static void EnsureSftpDirectory(SftpClient client, string remotePath)
        {
            var parts = remotePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            var currentPath = string.Empty;
            foreach (var part in parts)
            {
                currentPath += $"/{part}";
                if (!client.Exists(currentPath))
                {
                    client.CreateDirectory(currentPath);
                }
            }
        }

        private static async Task UploadFileToSftpAsync(SftpClient client, IFormFile file, string remotePath)
        {
            await using var stream = file.OpenReadStream();
            client.UploadFile(stream, remotePath);
        }

        private static void DeleteSftpFiles(IEnumerable<SavedPhotoInfo> files, SftpClient? client)
        {
            if (client == null)
            {
                return;
            }

            if (!client.IsConnected)
            {
                client.Connect();
            }

            foreach (var file in files)
            {
                if (!file.IsRemote)
                {
                    continue;
                }

                if (client.Exists(file.FilePath))
                {
                    client.DeleteFile(file.FilePath);
                }
            }
        }

        private sealed record SavedPhotoInfo(string FilePath, string StoredFileName, IFormFile File, bool IsRemote);
    }
}
