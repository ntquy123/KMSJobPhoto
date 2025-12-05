using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using erpsolution.dal.EF;
using erpsolution.entities;
using erpsolution.service.Interface;
using Microsoft.EntityFrameworkCore;
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
    }
}
