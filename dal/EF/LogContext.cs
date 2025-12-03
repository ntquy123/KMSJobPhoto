using System;
using erpsolution.dal.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace erpsolution.dal.EF
{
    public partial class LogContext : DbContext
    {
        public LogContext()
        {
        }

        public LogContext(DbContextOptions<LogContext> options) : base(options)
        {
        }

        public DbSet<ApiLogs> ApiLogs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ApiLogs>(entity =>
            {
                entity.ToTable("API_LOGS");

                entity.HasKey(e => new { e.LogId, e.TransDate });

                entity.Property(e => e.LogId)
                   .HasColumnName("LOG_ID").ValueGeneratedOnAdd();

                entity.Property(e => e.TransDate)
                   .HasColumnName("TRANS_DATE");

                entity.Property(e => e.Method)
               .HasColumnName("METHOD");

                entity.Property(e => e.MenuName)
                .HasColumnName("MENU_NAME");

                entity.Property(e => e.System)
              .HasColumnName("SYSTEM");

                entity.Property(e => e.ApiName)
              .HasColumnName("API_NAME");

                entity.Property(e => e.RequestJson)
                    .HasColumnName("REQUEST_JSON");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.CreateId)
                    .HasColumnName("CREATE_ID");

                entity.Property(e => e.Exception)
                   .HasColumnName("EXCEPTION");

                entity.Property(e => e.Message)
                   .HasColumnName("MESSAGE");
            });

             

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
