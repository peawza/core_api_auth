using Microsoft.AspNetCore.Identity;

namespace Application.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public string? CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool DeletedFlag { get; set; } = false;

        public DateTime? DeletedDate { get; set; }

        public string? DeletedBy { get; set; }
    }
}
