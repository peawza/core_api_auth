using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("ts_ImportLogDetail")]
    public partial class ts_ImportLogDetail
    {
        [Key]

        public Guid Id { get; set; }


        [StringLength(40)]
        public string JobLogId { get; set; } = null!;

        public int RowNo { get; set; }

        [StringLength(255)]
        public string ErrorDetail { get; set; } = null!;
    }


}
