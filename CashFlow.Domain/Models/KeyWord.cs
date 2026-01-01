using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("key_words")]
    public class KeyWord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("word_id")]
        public int WordId { get; set; }

        [Required]
        [Column("user_id")]
        public required int UserId { get; set; }

        [Required]
        [Column("category_id")]
        public required int CategoryId { get; set; }

        [Required]
        [Column("word")]
        public required string Word { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = null!;

        public Category Category { get; set; } = null!;
    }
}


//CREATE TABLE Key_words (
//    word_id SERIAL PRIMARY KEY,
//    user_id INTEGER NOT NULL REFERENCES Users(user_id),
//    category_id INTEGER NOT NULL REFERENCES Categories(category_id),
//    word VARCHAR(50) NOT NULL,
//    UNIQUE (user_id, word),
//    deleted_at TIMESTAMP
//);