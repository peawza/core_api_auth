using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models
{
    [Table("tb_Position")]
    [Index(nameof(PositionCode), IsUnique = true)]
    public class tb_Position
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string PositionCode { get; set; }

        [Required]
        public string PositionName { get; set; }
    }
}
