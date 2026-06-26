using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("ts_SystemConfig")]
    public class ts_SystemConfig
    {
        [Key]
        [Column("ConfigCode")]
        [StringLength(50)]
        public string ConfigCode { get; set; } = null!;

        [Column("ConfigDesc")]
        [StringLength(255)]
        public string? ConfigDesc { get; set; }

        [Column("ValueVarchar")]
        public string? ValueVarchar { get; set; }

        [Column("ValueVarchar2")]
        public string? ValueVarchar2 { get; set; }

        [Column("ValueVarchar3")]
        public string? ValueVarchar3 { get; set; }

        [Column("ValueInt")]
        public int? ValueInt { get; set; }

        [Column("ValueDate")]
        public DateTime? ValueDate { get; set; }

        [Column("ValueDecimal", TypeName = "decimal(18,2)")]
        public decimal? ValueDecimal { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;
    }

}
