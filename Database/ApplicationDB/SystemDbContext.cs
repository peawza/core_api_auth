using Application.Models;
using ApplicationDB.Models.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public class SystemDbContext : DbContext
    {
        public DbSet<ts_LocalizedMessages> LocalizedMessages { get; set; }
        public DbSet<ts_LocalizedResources> LocalizedResources { get; set; }
        public DbSet<ts_Module> Module { get; set; }
        public DbSet<ts_SubModule> SubModule { get; set; }
        public DbSet<ts_Screen> Screen { get; set; }
        public DbSet<ts_ScreenFunction> ScreenFunctions { get; set; }
        public DbSet<ts_ImportLog> tsImportLogs { get; set; }
        public DbSet<ts_ImportLogDetail> tsImportLogDetails { get; set; }
        public DbSet<ts_SystemConfig> TsSystemConfigs { get; set; }
        public DbSet<ts_URLConfig> TsUrlConfigs { get; set; }
        public DbSet<TbApiSmsLog> TbApiSmsLogs { get; set; }

        public SystemDbContext(DbContextOptions<SystemDbContext> options)
            : base(options)
        {
        }

        public static void InitialService(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SystemDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ts_LocalizedMessages>().ToTable("ts_LocalizedMessages");
            modelBuilder.Entity<ts_LocalizedResources>().ToTable("ts_LocalizedResources");
            modelBuilder.Entity<ts_Module>().ToTable("ts_Module");
            modelBuilder.Entity<ts_SubModule>().ToTable("ts_SubModule");
            modelBuilder.Entity<ts_Screen>().ToTable("ts_Screen");
            modelBuilder.Entity<ts_ScreenFunction>().ToTable("ts_ScreenFunction");
            modelBuilder.Entity<ts_ImportLog>().ToTable("ts_ImportLog");
            modelBuilder.Entity<ts_ImportLogDetail>().ToTable("ts_ImportLogDetail");
            modelBuilder.Entity<ts_SystemConfig>().ToTable("ts_SystemConfig");
            modelBuilder.Entity<ts_URLConfig>().ToTable("ts_URLConfig");
            modelBuilder.Entity<TbApiSmsLog>().ToTable("tb_API_SMS_Logs");
        }
    }
}