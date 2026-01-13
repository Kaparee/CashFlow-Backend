using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace CashFlow.Infrastructure.Repositories
{
    public class RecTransactionRepository : IRecTransactionRepository
    {
        private readonly CashFlowDbContext _context;

        public RecTransactionRepository(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecTransaction>> GetAccountRecTransactionsWithDetailsAsync(int userId, int accountId)
        {
            return await _context.RecTransactions
                .Include(rc => rc.Account)
                .Include(rc => rc.Category)
                .Where(rc => rc.UserId == userId && rc.AccountId == accountId && rc.DeletedAt == null)
                .ToListAsync();
        }

        public async Task AddAsync(RecTransaction recTransaction)
        {
            _context.RecTransactions.Add(recTransaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RecTransaction recTransaction)
        {
            _context.RecTransactions.Update(recTransaction);
            await _context.SaveChangesAsync();
        }

        public async Task<RecTransaction?> GetByIdWithDetailsAsync(int userId, int accountId, int recTransactionId)
        {
            return await _context.RecTransactions
                .Include(rc => rc.Account)
                .Include(rc => rc.Category)
                .Where(rc => rc.UserId == userId && rc.AccountId == accountId && rc.DeletedAt == null && rc.RecTransactionId == recTransactionId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<RecTransaction>> GetPendingRecurringTransactionsWithDetailsAsync(DateTime date)
        {
            return await _context.RecTransactions
                .Include(rc => rc.User)
                .Include(rc => rc.Account)
                .Include(rc => rc.Category)
                .Where(rc => rc.DeletedAt == null && rc.NextPaymentDate <= date)
                .ToListAsync();
        }

        public async Task<List<RecTransaction>> GetUpcomingRecurringTransactionsWithDetailsAsync(DateTime date)
        {
            return await _context.RecTransactions
                .Include(rc => rc.User)
                .Include(rc => rc.Account)
                .Include(rc => rc.Category)
                .Where(rc => rc.DeletedAt == null && rc.NextPaymentDate.Date == date.Date)
                .ToListAsync();
        }
    }
}