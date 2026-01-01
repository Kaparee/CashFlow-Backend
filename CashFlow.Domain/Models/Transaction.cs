using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("transactions")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("transaction_id")]
        public int TransactionId { get; set; }

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

        [Column("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("type")]
        public required string Type { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public User User { get; set; } = null!;

        public Account Account { get; set; } = null!;

        public Category Category { get; set; } = null!;
    }
}

//CREATE TABLE Transactions (
//    transaction_id SERIAL PRIMARY KEY,
//    user_id INTEGER NOT NULL REFERENCES Users(user_id),
//    account_id INTEGER NOT NULL REFERENCES Accounts(account_id),
//    category_id INTEGER REFERENCES Categories(category_id),
//    amount NUMERIC(10, 2) NOT NULL,
//    description TEXT,
//    date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
//    type VARCHAR(10) NOT NULL CHECK (type IN ('income', 'expense')),
//    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
//    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
//    deleted_at TIMESTAMP
//);
