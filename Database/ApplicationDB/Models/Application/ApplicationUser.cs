using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public bool FirstLoginFlag { get; set; }

        [Required]
        public bool ActiveFlag { get; set; }

        [Required]
        public bool SystemAdminFlag { get; set; }

        [Required]
        public bool OTPLogin { get; set; }

        public int? PasswordAge { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastUpdatePasswordDate { get; set; }

        public DateTime? ActiveDate { get; set; }

        public DateTime? InActiveDate { get; set; }

        public override string? UserName { get; set; }

        public override string? NormalizedUserName { get; set; }

        public override string? Email { get; set; }

        public override string? NormalizedEmail { get; set; }

        public override bool EmailConfirmed { get; set; }

        public override string? PasswordHash { get; set; }

        public override string? SecurityStamp { get; set; }

        public override string? ConcurrencyStamp { get; set; }

        public override string? PhoneNumber { get; set; }

        public override bool PhoneNumberConfirmed { get; set; }

        public override bool TwoFactorEnabled { get; set; }

        public override DateTimeOffset? LockoutEnd { get; set; }

        public override bool LockoutEnabled { get; set; }

        public override int AccessFailedCount { get; set; }

        public bool SendSMSOTP { get; set; }

        public bool SendEmailOTP { get; set; }


        public virtual ICollection<tb_PasswordHistory> PasswordHistories { get; set; }

        public ApplicationUser()
        {
            PasswordHistories = new HashSet<tb_PasswordHistory>();
        }
    }
}
