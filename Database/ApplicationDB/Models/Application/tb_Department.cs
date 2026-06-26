using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models
{
    [Table("tb_Department")]
    [Index(nameof(DepartmentCode), IsUnique = true)]
    public class tb_Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string DepartmentCode { get; set; }

        [Required]
        public string DepartmentName { get; set; }
    }
}
