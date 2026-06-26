using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("tb_API_SMS_Logs")]
    public partial class TbApiSmsLog
    {
        [Key]
        [Column("ID")]
        public Guid Id { get; set; }

        [Column("URL")]
        [StringLength(200)]
        public string? Url { get; set; }

        [Column("ResponseStatus")]
        [StringLength(50)]
        public string? ResponseStatus { get; set; }

        [Column("RequsestJSON")]
        public string? RequsestJson { get; set; }

        [Column("ResponseJSON")]
        public string? ResponseJson { get; set; }

        [Column("Error")]
        public string? Error { get; set; }

        [Column("CreateTime", TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
    }
}
