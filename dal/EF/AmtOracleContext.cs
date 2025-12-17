
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace erpsolution.dal.EF
{
    public partial class AmtOracleContext : DbContext
    {
        public AmtOracleContext()
        {
        }

        public AmtOracleContext(DbContextOptions<AmtOracleContext> options) : base(options)
        {
        }

        public AmtOracleContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<TCMUSMT> TCMUSMT { get; set; }
        public virtual DbSet<AuditTodoRow> AuditTodoRows { get; set; }
        public virtual DbSet<ZmMasMobileVersions> ZmMasMobileVersions { get; set; }
        public virtual DbSet<KmsAudresMst> KmsAudresMsts { get; set; }
        public virtual DbSet<KmsAudresPho> KmsAudresPhos { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TCMUSMT>().HasNoKey();
            modelBuilder.Entity<AuditTodoRow>().HasNoKey();
            modelBuilder.Entity<ZmMasMobileVersions>(entity =>
            {
                entity.HasKey(e => e.VersionId);
                entity.ToTable("ZM_MAS_MOBILE_VERSIONS");

                entity.Property(e => e.VersionId).HasColumnName("VERSION_ID");
                entity.Property(e => e.VersionName).HasColumnName("VERSION_NAME").HasMaxLength(50);
                entity.Property(e => e.ReleaseDate).HasColumnName("RELEASE_DATE");
                entity.Property(e => e.Description).HasColumnName("DESCRIPTION").HasMaxLength(255);
                entity.Property(e => e.UseYn).HasColumnName("USEYN").HasMaxLength(1);
            });
            modelBuilder.Entity<KmsAudresMst>(entity =>
            {
                entity.HasKey(e => new { e.AudplnNo, e.Catcode, e.CorrectionNo });
                entity.ToTable("KMS_AUDRES_MST");

                entity.Property(e => e.AudplnNo).HasColumnName("AUDPLN_NO").HasMaxLength(13).IsUnicode(false);
                entity.Property(e => e.Catcode).HasColumnName("CATCODE").HasMaxLength(13).IsUnicode(false);
                entity.Property(e => e.CorrectionNo).HasColumnName("CORRECTION_NO");
                entity.Property(e => e.DetailedFinding).HasColumnName("DETAILED_FINDING").HasMaxLength(4000).IsUnicode(false);
                entity.Property(e => e.LegalRequirements).HasColumnName("LEGAL_REQUIREMENTS").HasMaxLength(4000).IsUnicode(false);
                entity.Property(e => e.Instruction).HasColumnName("INSTRUCTION").HasMaxLength(4000).IsUnicode(false);
                entity.Property(e => e.Recommendations).HasColumnName("RECOMMENDATIONS").HasMaxLength(4000).IsUnicode(false);
                entity.Property(e => e.CorrectiveAction).HasColumnName("CORRECTIVE_ACTION").HasMaxLength(4000).IsUnicode(false);
                entity.Property(e => e.CorrectedDate).HasColumnName("CORRECTED_DATE");
                entity.Property(e => e.Crtid).HasColumnName("CRTID").HasMaxLength(8).IsUnicode(false);
                entity.Property(e => e.Crtdate).HasColumnName("CRTDATE");
                entity.Property(e => e.Uptid).HasColumnName("UPTID").HasMaxLength(8).IsUnicode(false);
                entity.Property(e => e.Uptdate).HasColumnName("UPTDATE");
            });

            modelBuilder.Entity<KmsAudresPho>(entity =>
            {
                entity.HasKey(e => new { e.AudplnNo, e.Catcode, e.CorrectionNo, e.PhoSeq });
                entity.ToTable("KMS_AUDRES_PHO");

                entity.Property(e => e.AudplnNo).HasColumnName("AUDPLN_NO").HasMaxLength(13).IsUnicode(false);
                entity.Property(e => e.Catcode).HasColumnName("CATCODE").HasMaxLength(13).IsUnicode(false);
                entity.Property(e => e.CorrectionNo).HasColumnName("CORRECTION_NO");
                entity.Property(e => e.PhoSeq).HasColumnName("PHO_SEQ");
                entity.Property(e => e.PhoFile).HasColumnName("PHO_FILE").HasMaxLength(1000).IsUnicode(false);
                entity.Property(e => e.PhoName).HasColumnName("PHO_NAME").HasMaxLength(1000).IsUnicode(false);
                entity.Property(e => e.PhoSize).HasColumnName("PHO_SIZE");
                entity.Property(e => e.PhoLink).HasColumnName("PHO_LINK").HasMaxLength(1000).IsUnicode(false);
                entity.Property(e => e.PhoDesc).HasColumnName("PHO_DESC").HasMaxLength(1000).IsUnicode(false);
                entity.Property(e => e.Crtid).HasColumnName("CRTID").HasMaxLength(8).IsUnicode(false);
                entity.Property(e => e.Crtdate).HasColumnName("CRTDATE");
                entity.Property(e => e.Uptid).HasColumnName("UPTID").HasMaxLength(8).IsUnicode(false);
                entity.Property(e => e.Uptdate).HasColumnName("UPTDATE");
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
