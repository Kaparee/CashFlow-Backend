using CashFlow.Application.Interfaces;
using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;
using BCrypt.Net;

namespace CashFlow.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyFetcher _currencyFetcher;
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyFetcher currencyFetcher, ICurrencyRepository currencyRepository)
        {
            _currencyFetcher = currencyFetcher;
            _currencyRepository = currencyRepository;
        }

        public async Task<List<CurrencyResponse>> GetAllCurrenciesAsync()
        {
            var currencies = await _currencyRepository.GetAllCurrenciesWithDetailsAsync();

            return currencies.Select(c => new CurrencyResponse
            {
                CurrencyCode = c.CurrencyCode,
                Name = c.Name,
                Symbol = c.Symbol
            }).ToList();
        }

        public async Task SyncRatesAsync()
        {
            var ratesFromNbp = await _currencyFetcher.FetchRatesAsync();

            if (ratesFromNbp == null || !ratesFromNbp.Any())
            {
                throw new Exception("Error while downloading new data from NBP");
            }

            var dbCurrencies = await _currencyRepository.GetAllCurrenciesWithDetailsAsync();

            if (!dbCurrencies.Any(c => c.CurrencyCode == "PLN"))
            {
                await _currencyRepository.AddAsync(new Currency
                {
                    CurrencyCode = "PLN",
                    Name = "Polski Z�oty",
                    RateToBase = 1.0m,
                    Symbol = "z�"
                });
            }

            foreach (var rate in ratesFromNbp)
            {
                var existingCurrency = dbCurrencies.FirstOrDefault(c => c.CurrencyCode == rate.Code);

                if (existingCurrency != null)
                {
                    existingCurrency.RateToBase = rate.Mid;
                    existingCurrency.LastUpdated = DateTime.UtcNow;
                    await _currencyRepository.UpdateAsync(existingCurrency);
                }
                else
                {
                    var newCurrency = new Currency
                    {
                        CurrencyCode = rate.Code,
                        Name = rate.Currency,
                        RateToBase = rate.Mid,
                        LastUpdated = DateTime.UtcNow,
                        Symbol = rate.Code
                    };
                    await _currencyRepository.AddAsync(newCurrency);
                }
            }

            await _currencyRepository.SaveChangesAsync();
        }
    }
}