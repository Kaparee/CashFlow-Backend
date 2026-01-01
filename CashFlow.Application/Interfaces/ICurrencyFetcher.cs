using CashFlow.Application.DTO.Externals;

namespace CashFlow.Application.Interfaces
{
    public interface ICurrencyFetcher
    {
        Task<List<CurrencyRate>> FetchRatesAsync();
    }
}