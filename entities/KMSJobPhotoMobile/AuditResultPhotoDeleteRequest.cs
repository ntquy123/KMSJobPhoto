using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace erpsolution.entities
{
    public class AuditResultPhotoDeleteRequest
    {
        /// <summary>
        /// The unique identifier of the Audit Plan (e.g., PLN202511-001).
        /// </summary>
        [Required]
        [DefaultValue("PLN202511-001")]
        public string AudplnNo { get; set; } = string.Empty;

        /// <summary>
        /// The category code related to the environment or safety issue.
        /// </summary>
        [Required]
        [DefaultValue("ENVI-002-0001")]
        public string Catcode { get; set; } = string.Empty;

        /// <summary>
        /// The sequence number of the correction action (e.g., 1, 2).
        /// </summary>
        [Required]
        [DefaultValue(1)]
        public decimal CorrectionNo { get; set; }

        /// <summary>
        /// Photos to delete (identified by photo sequence and stored file name).
        /// </summary>
        [Required]
        public List<AuditResultPhotoDeleteItem> Photos { get; set; } = new();

        /// <summary>
        /// Flag to use the SFTP test delete server.
        /// </summary>
        [DefaultValue(true)]
        public bool Test { get; set; } = true;
    }
}
