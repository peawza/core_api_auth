using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models
{
    [Table("tb_GroupPermission")]
    [PrimaryKey(nameof(GroupId), nameof(ScreenId))]
    public class tb_GroupPermission
    {
        [Required]
        public string GroupId { get; set; }

        [Required]
        public string ScreenId { get; set; }

        [Required]
        public int FunctionCode { get; set; }

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string CreateBy { get; set; } = "SYSTEM";

        [Required]
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string UpdateBy { get; set; } = "SYSTEM";

        public bool DeletedFlag { get; set; } = false;

        public DateTime? DeletedDate { get; set; }

        public string? DeletedBy { get; set; }
    }
}
