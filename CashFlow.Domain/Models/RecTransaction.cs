using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("rec_transactions")]
    public class RecTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("rec_transaction_id")]
        public int RecTransactionId { get; set; }

        [Required]
        [Column("type")]
        public required string Type { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Required]
        [Column("frequency")]
        public required string Frequency { get; set; }

        [Required]
        [Column("is_true")]
        public required bool IsTrue { get; set; } = true;

        [Required]
        [Column("start_date")]
        public required DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [Column("user_id")]
        public required int UserId { get; set; }

        [Required]
        [Column("account_id")]
        public required int AccountId { get; set; }

        [Required]
        [Column("category_id")]
        public required int CategoryId { get; set; }

        [Required]
        [Column("amount")]
        public required decimal Amount { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public User User { get; set; } = null!;

        public Account Account{ get; set; } = null!;

        public Category Category { get; set; } = null!;
    }
}