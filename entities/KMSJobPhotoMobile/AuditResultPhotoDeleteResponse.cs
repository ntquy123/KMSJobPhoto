using System;

namespace erpsolution.entities
{
    public class AuditResultPhotoDeleteResponse
    {
        public decimal PhotoSeq { get; set; }

        public string FileName { get; set; } = string.Empty;

        public DateTime DeletedAt { get; set; }
    }
}
