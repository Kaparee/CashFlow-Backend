using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CashFlow.Domain.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("first_name")]
        public required string FirstName { get; set; }
        [Required]
        [Column("last_name")]
        public required string LastName { get; set; }
        [Required]
        [Column("nickname")]
        public required string Nickname { get; set; }
        [Required]
        [Column("email")]
        public required string Email { get; set; }
        [Required]
        [Column("password_hash")]
        public required string PasswordHash { get; set; }

        [Column("is_verified")]
        public bool IsVerified { get; set; } = false;

        [Required]
        [Column("is_active")]
        public required bool IsActive { get; set; } = true;

        [Required]
        [Column("is_admin")]
        public required bool IsAdmin { get; set; } = false;

        [Column("photo_url")]
        public string? PhotoUrl { get; set; } = "default_user_url";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("verification_token")]
        public string? VerificationToken { get; set; }

        [Column("verified_at")]
        public DateTime? VerifiedAt { get; set; }

        [Column("password_reset_token")]
        public string? PasswordResetToken { get; set; }

        [Column("reset_token_expires_at")]
        public DateTime? ResetTokenExpiresAt { get; set; }

        [Column("new_email_pending")]
        public string? NewEmailPending { get; set; }

        [Column("email_change_token")]
        public string? EmailChangeToken { get; set; }

        [Column("email_change_token_expires_at")]
        public DateTime? EmailChangeTokenExpiresAt { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        public ICollection<KeyWord> KeyWords { get; set; } = new List<KeyWord>();

        public ICollection<Category> Categories { get; set; } = new List<Category>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public ICollection<RecTransaction> RecTransactions { get; set; } = new List<RecTransaction>();
    }
}


//CREATE TABLE Users (
//    user_id SERIAL PRIMARY KEY,
//    first_name VARCHAR(50) NOT NULL,
//    last_name VARCHAR(50) NOT NULL,
//    nickname VARCHAR(50) NOT NULL,
//    email VARCHAR(100) NOT NULL UNIQUE,
//    password_hash VARCHAR(255) NOT NULL,
//    is_verified BOOLEAN DEFAULT FALSE,
//    is_active BOOLEAN NOT NULL DEFAULT TRUE,
//    is_admin BOOLEAN NOT NULL DEFAULT FALSE,
//    photo_url VARCHAR(255) DEFAULT 'default_user_url',
//    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
//    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
//    deleted_at TIMESTAMP
//);
