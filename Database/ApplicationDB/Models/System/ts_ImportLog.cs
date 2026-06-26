using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("ts_ImportLog")]
    public partial class ts_ImportLog
    {
        [Key]

        [StringLength(40)]
        public string JobLogId { get; set; } = null!;


        [StringLength(20)]
        public string InterfaceCode { get; set; } = null!;


        [StringLength(100)]
        public string? InterfaceName { get; set; }


        [StringLength(100)]
        public string? FtpServername { get; set; }


        [StringLength(255)]
        public string? FileFolder { get; set; }


        [StringLength(255)]
        public string InterfaceFilename { get; set; } = null!;


        [StringLength(100)]
        public string JobStatus { get; set; } = null!;


        [StringLength(50)]
        public string ProcessBy { get; set; } = null!;


        public DateTime StartDatetime { get; set; }


        public DateTime FinishDatetime { get; set; }


        public int TotalRecord { get; set; }

        public string? Remark { get; set; }
    }
}
