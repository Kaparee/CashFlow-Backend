namespace CashFlow.Application.DTO.Responses
{
    public class AccountResponse
    {
        public int AccountId { get; set; }
        public required string Name { get; set; }
        public decimal Balance { get; set; }
        public required string CurrencyCode { get; set; }
        public string? PhotoUrl { get; set; }
    }
}