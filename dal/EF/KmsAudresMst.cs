using System;

namespace erpsolution.dal.EF
{
    public class KmsAudresMst
    {
        public string AudplnNo { get; set; } = null!;

        public string Catcode { get; set; } = null!;

        public decimal CorrectionNo { get; set; }

        public string? DetailedFinding { get; set; }

        public string? LegalRequirements { get; set; }

        public string? Instruction { get; set; }

        public string? Recommendations { get; set; }

        public string? CorrectiveAction { get; set; }

        public DateTime? CorrectedDate { get; set; }

        public string? Crtid { get; set; }

        public DateTime? Crtdate { get; set; }

        public string? Uptid { get; set; }

        public DateTime? Uptdate { get; set; }
    }
}
