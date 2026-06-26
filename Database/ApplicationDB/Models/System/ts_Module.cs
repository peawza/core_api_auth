using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.System
{
    [Table("ts_Module")]
    public partial class ts_Module
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string ModuleCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string ModuleNameEN { get; set; }

        [Required]
        [MaxLength(100)]
        public string ModuleNameTH { get; set; }

        [Required]
        public int Seq { get; set; }

        [MaxLength(50)]

        public string? IconClass { get; set; }
    }
}
