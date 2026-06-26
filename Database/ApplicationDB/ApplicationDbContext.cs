using Application.Models;
using ApplicationDB.Models.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Application
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<tb_Department> Departments { get; set; }
        public DbSet<tb_GroupPermission> GroupPermissions { get; set; }
        public DbSet<tb_PasswordHistory> PasswordHistorys { get; set; }
        public DbSet<tb_Position> Positions { get; set; }
        public DbSet<tb_UserInfo> UserInfos { get; set; }
        public DbSet<tb_UserRole> UserRoles { get; set; }
        public DbSet<tb_LoginOtp> LoginOtps { get; set; }


        public ApplicationDbContext(
                DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public static void InitialService(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ ปรับให้ใช้ table ธรรมดา ไม่มี schema
            builder.Entity<ApplicationUser>().ToTable("tb_User");
            builder.Entity<IdentityRole>().ToTable("tb_Role");

            builder.Entity<ApplicationRole>(b =>
            {
                b.ToTable("tb_Role");

                b.Property(r => r.Description).HasColumnName("Description").HasMaxLength(256);
                b.Property(r => r.IsActive).HasColumnName("IsActive");
                b.Property(r => r.CreateBy).HasColumnName("CreateBy").HasMaxLength(20);
                b.Property(r => r.CreateDate).HasColumnName("CreateDate");
                b.Property(r => r.UpdateBy).HasColumnName("UpdateBy").HasMaxLength(20);
                b.Property(r => r.UpdateDate).HasColumnName("UpdateDate");
                b.Property(r => r.DeletedFlag).HasColumnName("DeletedFlag");
                b.Property(r => r.DeletedDate).HasColumnName("DeletedDate");
                b.Property(r => r.DeletedBy).HasColumnName("DeletedBy").HasMaxLength(50);
            });

            builder.Entity<IdentityUserClaim<string>>().ToTable("tb_UserClaim");
            //builder.Entity<IdentityUserRole<string>>().ToTable("tm_user_role");
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("tb_UserRole");

                entity.HasKey(x => new { x.UserId, x.RoleId });

                entity.Property(x => x.UserId).HasMaxLength(450);
                entity.Property(x => x.RoleId).HasMaxLength(450);

                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ApplicationRole>()
                    .WithMany()
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            builder.Entity<IdentityUserLogin<string>>().ToTable("tb_UserLogin");
            builder.Entity<IdentityUserToken<string>>().ToTable("tb_UserToken");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("tb_RoleClaim");

            builder.Entity<ApplicationUser>()
                .HasMany(t => t.PasswordHistories)
                .WithOne(t => t.User)
                .HasForeignKey("Id");

            builder.Entity<tb_Department>().ToTable("tb_Department");
            builder.Entity<tb_Position>().ToTable("tb_Position");
            builder.Entity<tb_GroupPermission>().ToTable("tb_GroupPermission");
            builder.Entity<tb_PasswordHistory>().ToTable("tb_PasswordHistory");
            builder.Entity<tb_UserInfo>().ToTable("tb_UserInfo");
            builder.Entity<tb_LoginOtp>().ToTable("tb_LoginOtp");
        }
    }
}
