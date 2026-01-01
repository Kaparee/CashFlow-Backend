using CashFlow.Application.Interfaces;
using CashFlow.Application.DTO.Externals;
using System.Net.Http.Json;

namespace CashFlow.Infrastructure.ExternalServices
{
    public class NbpTableDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("rates")]
        public List<CurrencyRate> Rates { get; set; } = new();
    }

    public class CurrencyFetcher : ICurrencyFetcher
    {
        private readonly HttpClient _httpClient;

        public CurrencyFetcher(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CurrencyRate>> FetchRatesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<NbpTableDto>>("http://api.nbp.pl/api/exchangerates/tables/A/?format=json");

            if (result == null || result.Count == 0)
            {
                return new List<CurrencyRate>();
            }

            return result[0].Rates;
        }


    }
    
}