using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace CashFlow.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly CashFlowDbContext _context;

        public TransactionRepository(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAccountTransactionsWithDetailsAsync(int userId, int accountId)
        {
            return await _context.Transactions
                .Include(c => c.Category)
                .Where(a => a.AccountId == accountId && a.UserId == userId && a.DeletedAt == null)
                .ToListAsync();
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<Transaction?> GetTransactionInfoByIdWithDetailsAsync(int userId, int transactionId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.TransactionId == transactionId && t.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasTransactionsAsync(int userId, int categoryId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.CategoryId == categoryId && t.DeletedAt == null)
                .AnyAsync();
        }

        public async Task<List<Transaction>> GetTransactionsInfoByCategoryIdWithDetailsAsync(int userId, int categoryId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.CategoryId == categoryId && t.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<decimal> GetCategorySpendingsAsync(int userId, int categoryId, DateTime start, DateTime end)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.CategoryId == categoryId &&
                            t.Type == "expense" &&
                            t.DeletedAt == null &&
                            t.Date >= start && t.Date <= end)
                .SumAsync(t => t.Amount);
        }
    }
}