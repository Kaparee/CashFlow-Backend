namespace CashFlow.Application.DTO.Requests
{
    public class NewAccountRequest
    {
        public string? Name { get; set; }
        public decimal? Balance { get; set; }
        public string? CurrencyCode { get; set; }
        public string? PhotoUrl { get; set; }
    }
}