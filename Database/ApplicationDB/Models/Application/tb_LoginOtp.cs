using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.Application
{
    [Table("tb_LoginOtp")]
    public class tb_LoginOtp
    {
        [Key]
        [Column("OtpId")]
        public Guid OtpId { get; set; }

        [Column("UserId")]
        public Guid? UserId { get; set; }

        [Required]
        [Column("LoginIdentifier")]
        [StringLength(100)]
        public string LoginIdentifier { get; set; } = null!;

        [Required]
        [Column("OtpCodeHash")]
        [StringLength(200)]
        public string OtpCodeHash { get; set; } = null!;

        [Required]
        [Column("Channel")]
        [StringLength(20)]
        public string Channel { get; set; } = null!;   // SMS / EMAIL / APP

        [Column("ReferenceNo")]
        [StringLength(50)]
        public string? ReferenceNo { get; set; }

        [Required]
        [Column("ExpireAt")]
        public DateTime ExpireAt { get; set; }

        [Required]
        [Column("SentAt")]
        public DateTime SentAt { get; set; }

        [Column("VerifiedAt")]
        public DateTime? VerifiedAt { get; set; }

        [Required]
        [Column("Status")]
        [StringLength(20)]
        public string Status { get; set; } = null!;    // PENDING / VERIFIED / EXPIRED / CANCELLED

        [Required]
        [Column("AttemptCount")]
        public int AttemptCount { get; set; }

        [Required]
        [Column("MaxAttempt")]
        public int MaxAttempt { get; set; }

        [Column("RequestIp")]
        [StringLength(50)]
        public string? RequestIp { get; set; }

        [Column("UserAgent")]
        [StringLength(256)]
        public string? UserAgent { get; set; }

        [Required]
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("CreatedBy")]
        [StringLength(50)]
        public string CreatedBy { get; set; } = "SYSTEM";

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("UpdatedBy")]
        [StringLength(50)]
        public string? UpdatedBy { get; set; }
    }
}
