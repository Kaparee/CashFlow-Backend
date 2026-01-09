using CashFlow.Domain.Models;


namespace CashFlow.Application.Repositories
{
    public interface ICurrencyRepository
    {
        Task<List<Currency>> GetAllCurrenciesWithDetailsAsync();
        Task AddAsync(Currency currency);
        Task UpdateAsync(Currency currency);
        Task SaveChangesAsync();
        Task<Currency?> GetCurrencyByCodeAsync(string code);
    }
}