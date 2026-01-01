using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace CashFlow.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CashFlowDbContext _context;

        public AccountRepository(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountByIdAsync(int userId, int accountId)
        {
            return await _context.Accounts
                .Where(x => x.DeletedAt == null)
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.UserId == userId);
        }

        public async Task AddAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> isAccountCreated(int userId, string name)
        {
            return await _context.Accounts
                .AnyAsync(c => c.UserId == userId && c.Name == name && c.DeletedAt == null);
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Account>> GetUserAccountsWithDetailsAsync(int userId)
        {
            return await _context.Accounts
                .Where(x => x.DeletedAt == null)
                .Where(u => u.UserId == userId && u.IsActive == true)
                .ToListAsync();
        }
    }
}