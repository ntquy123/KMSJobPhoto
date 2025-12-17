using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace erpsolution.entities
{
    public class AuditResultPhotoUploadRequest
    {
        [Required]
        public string AudplnNo { get; set; } = string.Empty;

        [Required]
        public string Catcode { get; set; } = string.Empty;

        [Required]
        public decimal CorrectionNo { get; set; }

        [Required]
        public string CorrectiveAction { get; set; } = string.Empty;

        public string? PhotoDescription { get; set; }

        [Required]
        public IFormFile Photo { get; set; } = default!;
    }
}
