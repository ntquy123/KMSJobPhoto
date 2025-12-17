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
using Oracle.ManagedDataAccess.Client;
using service.Common.Base;

namespace erpsolution.service.KMSJobPhotoMobile
{
    public class AuditResultRegistrationService : ServiceBase<AuditTodoRow>, IAuditResultRegistrationService
    {
        public AuditResultRegistrationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string PrimaryKey => string.Empty;

        private const string AuditImageRootPath = "/home/data/PKKMS/upload/audit/img";

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

        public async Task<AuditResultPhotoUploadResponse> UploadPhotoAsync(AuditResultPhotoUploadRequest request)
        {
            if (request.Photo == null || request.Photo.Length == 0)
            {
                throw new ArgumentException("Photo is required", nameof(request.Photo));
            }

            var folderName = $"{request.AudplnNo}-{request.Catcode}-{request.CorrectionNo}";
            var folderPath = Path.Combine(AuditImageRootPath, folderName);
            Directory.CreateDirectory(folderPath);

            var fileName = GenerateUniqueFileName(request.Photo.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            await SaveFileAsync(request.Photo, filePath);

            IDbContextTransaction? transaction = null;
            try
            {
                transaction = await _amtContext.Database.BeginTransactionAsync();
                var response = await SavePhotoMetadataAsync(request, fileName, folderName);
                await transaction.CommitAsync();
                return response;
            }
            catch
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                throw;
            }
            finally
            {
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

        private async Task<AuditResultPhotoUploadResponse> SavePhotoMetadataAsync(AuditResultPhotoUploadRequest request, string storedFileName, string folderName)
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

            var currentUser = _currentUser?.UserName ?? "SYSTEM";
            var now = DateTime.Now;

            auditResult.CorrectiveAction = request.CorrectiveAction;
            auditResult.CorrectedDate = now;
            auditResult.Uptid = currentUser;
            auditResult.Uptdate = now;

            var nextSeq = await GetNextPhotoSequenceAsync(request);
            var relativePath = Path.Combine(folderName, storedFileName).Replace("\\", "/");
            var photoEntity = new KmsAudresPho
            {
                AudplnNo = request.AudplnNo,
                Catcode = request.Catcode,
                CorrectionNo = request.CorrectionNo,
                PhoSeq = nextSeq,
                PhoFile = storedFileName,
                PhoName = request.Photo.FileName,
                PhoSize = request.Photo.Length,
                PhoLink = relativePath,
                PhoDesc = request.PhotoDescription,
                Crtid = currentUser,
                Crtdate = now,
                Uptid = currentUser,
                Uptdate = now
            };

            _amtContext.KmsAudresPhos.Add(photoEntity);
            await _amtContext.SaveChangesAsync();

            return new AuditResultPhotoUploadResponse
            {
                AudplnNo = request.AudplnNo,
                Catcode = request.Catcode,
                CorrectionNo = request.CorrectionNo,
                PhotoSeq = nextSeq,
                FileName = storedFileName,
                FileLink = relativePath,
                PhotoDescription = request.PhotoDescription,
                UploadedAt = now
            };
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

        private static string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var safeName = Path.GetFileNameWithoutExtension(originalFileName);
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                safeName = safeName.Replace(invalidChar, '_');
            }

            return $"{safeName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
        }
    }
}
