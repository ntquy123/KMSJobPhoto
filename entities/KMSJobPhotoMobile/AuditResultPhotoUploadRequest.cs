using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace erpsolution.entities
{
    public class AuditResultPhotoUploadRequest
    {
        /// <summary>
        /// The unique identifier of the Audit Plan (e.g., PLN202511-001).
        /// </summary>
        [Required]
        [DefaultValue("PLN202511-001")] // Swagger will pre-fill this value
        public string AudplnNo { get; set; } = string.Empty;

        /// <summary>
        /// The category code related to the environment or safety issue.
        /// </summary>
        [Required]
        [DefaultValue("ENVI-002-0001")] // Swagger will pre-fill this value
        public string Catcode { get; set; } = string.Empty;

        /// <summary>
        /// The sequence number of the correction action (e.g., 1, 2).
        /// </summary>
        [Required]
        [DefaultValue(1)] // Swagger will pre-fill this value
        public decimal CorrectionNo { get; set; }

        /// <summary>
        /// Description of the action taken to resolve the issue.
        /// </summary>
        [Required]
        [DefaultValue("I’ve cleaned it properly")] // Swagger will pre-fill this value
        public string CorrectiveAction { get; set; } = string.Empty;

        /// <summary>
        /// Optional description or note for the uploaded photo.
        /// </summary>
        [DefaultValue("No need send only for test")] // Swagger will pre-fill this value
        public string? PhotoDescription { get; set; }

        /// <summary>
        /// The image file to prove the corrective action (Max 5MB).
        /// </summary>
        [Required]
        [DataType(DataType.Upload)] // Renders a file upload button in Swagger
        public IFormFile Photo { get; set; } = default!;
    }
}
