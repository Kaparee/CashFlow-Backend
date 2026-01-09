using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [Column("user_id")]
        public required int UserId { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Column("color")]
        public string? Color { get; set; } = "#808080";

        [Column("icon")]
        public string? Icon { get; set; }

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

        public ICollection<KeyWord> KeyWords { get; set; } = new List<KeyWord>();

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public ICollection<RecTransaction> RecTransactions { get; set; } = new List<RecTransaction>();

        public ICollection<Limit> Limits { get; set; } = new List<Limit>();
    }
}