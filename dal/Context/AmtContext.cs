using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erpsolution.dal.EF;

namespace erpsolution.dal.Context
{
    public partial class AmtContext : AmtOracleContext
    {
        //IOptions<ConnectionSetting> _settings;
        DbContextOptions<AmtContext> _options;
        //public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        //{
        //    builder
        //        .AddFilter((category, level) =>
        //            category == DbLoggerCategory.Database.Command.Name
        //            //&& category == DbLoggerCategory.Query.Name
        //            //&& category == DbLoggerCategory.Update.Name
        //            //&& category == DbLoggerCategory.Database.Transaction.Name
        //            //&& level == LogLevel.Debug
        //            )
        //        // .AddConsole()
        //        .AddDebug();
        //});
        //public AmtContext(IOptions<ConnectionSetting> settings)
        //{
        //    _settings = settings;
        //}

        public AmtContext(DbContextOptions<AmtContext> options) : base(options)
        {
            _options = options;
        }

        public async Task<int> ExecuteNonQueryAsync(string query, CommandType commandType = CommandType.Text, List<OracleParameter> parameters = null)
        {
            DbConnection connection = Database.GetDbConnection();
            try
            {
                connection.Open();
                DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                using (var cmd = dbFactory.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = commandType;

                    cmd.CommandText = query;
                    if (parameters != null)
                    {
                        foreach (var item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }

                    // Debug.WriteLine(cmd.OracleDumper());
                    var rs = await cmd.ExecuteNonQueryAsync();
                    //var rs1 =await cmd.ExecuteScalarAsync();

                    return rs;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return await Task.FromResult(-1);
                throw;
            }
            finally
            {

                connection.Close();
            }
        }
        public DataSet ExcuteDataSet(string query, CommandType commandType = CommandType.Text, List<OracleParameter> parameters = null)
        {
            DataSet ds = new DataSet();
            DbConnection connection = Database.GetDbConnection();
            try
            {
                DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                using (var cmd = dbFactory.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = commandType;

                    cmd.CommandText = query;
                    if (parameters != null)
                    {
                        foreach (var item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }
                    //Debug.WriteLine(cmd.OracleDumper());
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
                throw;
            }
            finally
            {

                connection.Close();
            }
            return ds;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer(_settings.Value.MsSQLConnection);
            //}
           // optionsBuilder.UseLoggerFactory(loggerFactory) //tie-up DbContext with LoggerFactory object
           //.EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }
        //
        // Summary:
        //     Override this method to further configure the model that was discovered by convention
        //     from the entity types exposed in Microsoft.EntityFrameworkCore.DbSet`1 properties
        //     on your derived context. The resulting model may be cached and re-used for subsequent
        //     instances of your derived context.
        //
        // Parameters:
        //   modelBuilder:
        //     The builder being used to construct the model for this context. Databases (and
        //     other extensions) typically define extension methods on this object that allow
        //     you to configure aspects of the model that are specific to a given database.
        //
        // Remarks:
        //     If a model is explicitly set on the options for this context (via Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel))
        //     then this method will not be run.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<vw_getgeneral>(entity =>
            //{
            //    entity.HasKey(e => new { e.COMPANY_ID,e.CATE_CD,e.GEN_CD});
            //});

            base.OnModelCreating(modelBuilder);

        }
    }
}
