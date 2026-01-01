using CashFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Principal;

namespace CashFlow.Infrastructure.Data
{
    public class CashFlowDbContext : DbContext
    {
        public CashFlowDbContext(DbContextOptions<CashFlowDbContext> options)
            : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Limit> Limits { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RecTransaction> RecTransactions { get; set; }
        public DbSet<KeyWord> KeyWords { get; set; }
        public DbSet<Notification> Notifications { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Currency>().ToTable("currencies");

            modelBuilder.Entity<Account>().ToTable("accounts");

            modelBuilder.Entity<Category>().ToTable("categories");

            modelBuilder.Entity<Limit>().ToTable("limits");

            modelBuilder.Entity<Transaction>().ToTable("transactions");

            modelBuilder.Entity<RecTransaction>().ToTable("rec_transactions");

            modelBuilder.Entity<KeyWord>().ToTable("key_words");
            modelBuilder.Entity<KeyWord>()
            .HasIndex(k => new { k.UserId, k.Word })
            .IsUnique();

            modelBuilder.Entity<Notification>().ToTable("notifications");

            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .IsRequired();


            modelBuilder.Entity<KeyWord>()
                .HasOne(k => k.Category)
                .WithMany(c => c.KeyWords)
                .HasForeignKey(k => k.CategoryId)
                .IsRequired();

            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .IsRequired();

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Currency)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CurrencyCode)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .IsRequired();

            modelBuilder.Entity<RecTransaction>()
                .HasOne(rt => rt.Category)
                .WithMany(c => c.RecTransactions)
                .HasForeignKey(rt => rt.CategoryId)
                .IsRequired();
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .IsRequired();
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .IsRequired();
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .IsRequired();
            modelBuilder.Entity<RecTransaction>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RecTransactions)
                .HasForeignKey(rt => rt.UserId)
                .IsRequired();
            modelBuilder.Entity<RecTransaction>()
                .HasOne(rt => rt.Account)
                .WithMany(a => a.RecTransactions)
                .HasForeignKey(rt => rt.AccountId)
                .IsRequired();
            modelBuilder.Entity<Limit>()
                .HasOne(l => l.Category)
                .WithMany(c => c.Limits)
                .HasForeignKey(l => l.CategoryId)
                .IsRequired();
            base.OnModelCreating(modelBuilder);

           
        }
    }
}
