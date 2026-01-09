using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("notifications")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Required]
        [Column("user_id")]
        public required int UserId { get; set; }

        [Required]
        [Column("email")]
        public required string Email { get; set; }

        [Required]
        [Column("subject")]
        public required string Subject { get; set; }

        [Required]
        [Column("body")]
        public required string Body { get; set; }

        [Column("type")]
        public string Type { get; set; } = "info";

        [Column("sent_at")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [Column("status")]
        public string Status { get; set; } = "sent";

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public User User { get; set; } = null!;
    }
}