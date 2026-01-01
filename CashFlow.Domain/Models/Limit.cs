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

        public Category Category { get; set; } = null!;

    }
}


//CREATE TABLE Limits (
//    limit_id SERIAL PRIMARY KEY,
//    category_id INTEGER NOT NULL REFERENCES Categories(category_id),
//    name VARCHAR(50) NOT NULL,
//    value NUMERIC(10, 2) NOT NULL,
//    description TEXT,
//    start_date DATE NOT NULL,
//    end_date DATE NOT NULL,
//    CONSTRAINT check_dates CHECK (end_date >= start_date),
//    deleted_at TIMESTAMP
//);