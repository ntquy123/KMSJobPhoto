using System;

namespace erpsolution.dal.EF
{
    public class KmsAudresPho
    {
        public string AudplnNo { get; set; } = null!;

        public string Catcode { get; set; } = null!;

        public decimal CorrectionNo { get; set; }

        public decimal PhoSeq { get; set; }

        public string? PhoFile { get; set; }

        public string? PhoName { get; set; }

        public decimal? PhoSize { get; set; }

        public string? PhoLink { get; set; }

        public string? PhoDesc { get; set; }

        public string? Crtid { get; set; }

        public DateTime? Crtdate { get; set; }

        public string? Uptid { get; set; }

        public DateTime? Uptdate { get; set; }
    }
}
