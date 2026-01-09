using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("currencies")]
    public class Currency
    {
        [Key]
        [Column("currency_code")]
        public required string CurrencyCode { get; set; }

        [Required]
        [Column("name")]
        public required string Name { get; set; }
        
        [Column("symbol")]
        public string? Symbol { get; set; }
        
        [Column("rate_to_base")]
        public decimal RateToBase { get; set; }

        [Column("base_currency")]
        public string? BaseCurrency { get; set; } = "PLN";

        [Column("last_updated")]
        public DateTime? LastUpdated { get; set; } = DateTime.UtcNow;


        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
