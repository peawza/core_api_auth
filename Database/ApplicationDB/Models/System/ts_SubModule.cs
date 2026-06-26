using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("ts_SubModule")]
    public partial class ts_SubModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]

        public string SubModuleCode { get; set; }

        [Required]
        [MaxLength(100)]

        public string SubModuleNameEN { get; set; }

        [Required]
        [MaxLength(100)]

        public string SubModuleNameTH { get; set; }

        [Required]

        public int Seq { get; set; }

        [MaxLength(50)]

        public string? IconClass { get; set; }
    }

}
