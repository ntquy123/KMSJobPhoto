using System.Collections.Generic;
using System.Threading.Tasks;
using erpsolution.dal.EF;
using erpsolution.entities;

namespace erpsolution.service.Interface
{
    public interface IAuditResultRegistrationService
    {
        new string PrimaryKey { get; }

        Task<List<AuditTodoRow>> GetTodoListAsync(AmtTodoRequest request);

        Task<List<AuditResultPhotoUploadResponse>> UploadPhotoAsync(AuditResultPhotoUploadRequest request, string? baseUrl = null);

        Task<List<AuditResultPhotoListResponse>> GetPhotoListAsync(AuditResultPhotoListRequest request, string? baseUrl = null);
    }
}
