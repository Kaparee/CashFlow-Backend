using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
	public interface ICurrencyService
	{
		Task<List<CurrencyResponse>> GetAllCurrenciesAsync();
		Task SyncRatesAsync();
        Task<decimal> GetExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode);
    }
}