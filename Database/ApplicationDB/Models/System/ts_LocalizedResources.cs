using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models
{

    [Table("ts_LocalizedResources")]
    [PrimaryKey(nameof(ScreenCode), nameof(ObjectID))]
    public class ts_LocalizedResources
    {
        [Key]
        [Column(TypeName = "VARCHAR(20)")]
        public string ScreenCode { get; set; } = null!;

        [Key]
        [Column(TypeName = "VARCHAR(50)")]
        public string ObjectID { get; set; } = null!;

        [Column(TypeName = "VARCHAR(256)")]
        public string? ResourcesEN { get; set; }

        [Column(TypeName = "VARCHAR(256)")]
        public string? ResourcesTH { get; set; }

        [Column(TypeName = "VARCHAR(256)")]
        public string? Remark { get; set; }
        public DateTime? CreateDate { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string? CreateBy { get; set; }
    }
}
