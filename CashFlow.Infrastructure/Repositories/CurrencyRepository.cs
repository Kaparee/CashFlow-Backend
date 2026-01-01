using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CashFlowDbContext _context;

        public CurrencyRepository(CashFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Currency>> GetAllCurrenciesWithDetailsAsync()
        {
            return await _context.Currencies
                .ToListAsync();
        }

        public async Task AddAsync(Currency currency)
        {
            await _context.Currencies.AddAsync(currency);
        }

        public Task UpdateAsync(Currency currency)
        {
            _context.Currencies.Update(currency);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}