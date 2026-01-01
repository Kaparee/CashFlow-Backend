using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
	public interface ICurrencyService
	{
		Task<List<CurrencyResponse>> GetAllCurrenciesAsync();
		Task SyncRatesAsync();
    }
}