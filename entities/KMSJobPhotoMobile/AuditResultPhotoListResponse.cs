using System;

namespace erpsolution.entities
{
    public class AuditResultPhotoListResponse
    {
        public string AudplnNo { get; set; } = string.Empty;

        public string Catcode { get; set; } = string.Empty;

        public decimal CorrectionNo { get; set; }

        public decimal PhotoSeq { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string PhotoUrl { get; set; } = string.Empty;

        public string? PhotoDescription { get; set; }

        public DateTime? UploadedAt { get; set; }
    }
}
