using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace CashFlow.Domain.Models
{
    [Table("accounts")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("user_id")]
        public required int UserId { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Column("balance", TypeName = "decimal(22,2)")]
        public decimal Balance { get; set; } = 0.00m;

        [Required]
        [Column("currency_code")]
        public required string CurrencyCode { get; set; }

        [Required]
        [Column("is_active")]
        public required bool IsActive { get; set; } = true;

        [Column("photo_url")]
        public string PhotoUrl { get; set; } = "default_account_url";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }


        public User User { get; set; } = null!;

        public Currency Currency { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public ICollection<RecTransaction> RecTransactions { get; set; } = new List<RecTransaction>();
    }
}
