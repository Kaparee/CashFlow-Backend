using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace CashFlow.Infrastructure.Repositories
{
    public class LimitRepository : ILimitRepository
    {
        private readonly CashFlowDbContext _context;

        public LimitRepository(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Limit>> GetLimitsForCategoryAsync(int categoryId)
        {
            return await _context.Limits
                .Include(l => l.Category)
                .Where(l => l.CategoryId == categoryId && l.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Limit?> GetLimitByIdAsync(int limitId, int userId)
        {
            return await _context.Limits
                .Include(l => l.Category)
                .FirstOrDefaultAsync(l => l.LimitId == limitId && l.Category.UserId == userId && l.DeletedAt == null);
        }

        public async Task AddAsync(Limit limit)
        {
            _context.Limits.Add(limit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Limit limit)
        {
            _context.Limits.Update(limit);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Limit>> GetUserLimitsAsync(int userId)
        {
            return await _context.Limits
                .Include(l => l.Category)
                .Where(l => l.Category.UserId == userId && l.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<List<Limit>> GetLimitsByAccountIdAsync(int accountId)
        {
            return await _context.Limits
                .Where(l => l.AccountId == accountId && l.DeletedAt == null)
                .ToListAsync();
        }
    }
}