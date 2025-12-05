
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

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
