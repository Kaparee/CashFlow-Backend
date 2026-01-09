using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace CashFlow.Domain.Models
{
    [Table("limits")]
    public class Limit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("limit_id")]
        public int LimitId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }

        [Required]
        [Column("value")]
        public required decimal Value { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("start_date")]
        public required DateTime StartDate { get; set; }

        [Required]
        [Column("end_date")]
        public required DateTime EndDate { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("account_id")]
        public int? AccountId { get; set; }

        public Category Category { get; set; } = null!;

        public Account? Account { get; set; }

    }
}