using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace erpsolution.dal.EF
{
    public class AuditTodoRow
    {
        [Column("AUDPLN_NO")]
        public string? AudplnNo { get; set; }

        [Column("CATCODE")]
        public string? Catcode { get; set; }

        [Column("CORRECTION_NO")]
        public string? CorrectionNo { get; set; }

        [Column("STATUS")]
        public string? Status { get; set; }

        [Column("STATUS_NAME")]
        public string? StatusName { get; set; }

        [Column("DTL_COMPLETION_DATE")]
        public string? CompletionDate { get; set; }

        [Column("PICID")]
        public string? PicId { get; set; }

        [Column("PICNAME")]
        public string? PicName { get; set; }

        [Column("TARGET_DATE")]
        public string? TargetDate { get; set; }

        [Column("DETAILED_FINDING")]
        public string? DetailedFinding { get; set; }

        [Column("CORRECTIVE_ACTION")]
        public string? CorrectiveAction { get; set; }

        [Column("CORPORATION")]
        public string? Corporation { get; set; }

        [Column("CORPORATIONNAME")]
        public string? CorporationName { get; set; }

        [Column("DEPT")]
        public string? Department { get; set; }

        [Column("DEPTNAME")]
        public string? DepartmentName { get; set; }

        [Column("CORRECTED_DATE")]
        public string? CorrectedDate { get; set; }

        [Column("PHOTOCNT")]
        public decimal? PhotoCount { get; set; }

        [Column("UPLOAD_DATE")]
        public string? UploadDate { get; set; }
    }
}
