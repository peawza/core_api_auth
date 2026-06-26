using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("ts_ScreenFunction")]
    public partial class ts_ScreenFunction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int FunctionId { get; set; }

        [Required]

        public int FunctionCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string FunctionName { get; set; } = null!;
    }
}

