using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("ts_Screen")] // ถ้าใช้ table จริงชื่อ mes_screen ให้เปลี่ยนกลับ
    public partial class ts_Screen
    {
        [Key]

        [StringLength(50)]
        public string ScreenId { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string NameEN { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string NameTH { get; set; } = null!;

        [Column(TypeName = "varchar(100)")]
        public string? SupportDeviceType { get; set; }

        [Required]

        public int FunctionCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ModuleCode { get; set; } = null!;

        [Column(TypeName = "varchar(50)")]
        public string? SubModuleCode { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? IconClass { get; set; }

        [Required]

        public bool MainMenuFlag { get; set; }

        [Required]

        public bool PermissionFlag { get; set; }

        [Required]

        public int Seq { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? PageTitleNameEN { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? PageTitleNameTH { get; set; }
    }
}
