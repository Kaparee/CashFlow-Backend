using System.Text.Json.Serialization;

namespace CashFlow.Application.DTO.Externals
{
    public class CurrencyRate
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("mid")]
        public decimal Mid { get; set; }
    }
}