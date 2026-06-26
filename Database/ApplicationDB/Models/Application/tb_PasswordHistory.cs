using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models
{
    [Table(name: "tb_PasswordHistory")]
    [PrimaryKey(nameof(Id), nameof(HistoryId))]
    public class tb_PasswordHistory
    {

        [Comment("User ID (Main Key)")]
        public string Id { get; set; }
        public int HistoryId { get; set; }
        public DateTime HistoryDate { get; set; }
        public string PasswordHash { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
