using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("ts_URLConfig")]
    public class ts_URLConfig
    {
        [Key]
        [Column("Code")]
        [StringLength(50)]
        public string Code { get; set; } = null!;

        [Column("Name")]
        [StringLength(200)]
        public string? Name { get; set; }

        [Column("URL")]
        [StringLength(200)]
        public string? URL { get; set; }

        [Column("Remark")]
        [StringLength(200)]
        public string? Remark { get; set; }

        [Column("UpdateUserID")]
        [StringLength(50)]
        public string? UpdateUserID { get; set; }

        [Column("UpdateDateTime")]
        public DateTime? UpdateDateTime { get; set; }
    }

}
