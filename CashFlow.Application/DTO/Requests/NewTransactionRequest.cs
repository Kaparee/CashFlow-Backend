namespace CashFlow.Application.DTO.Requests
{
    public class NewTransactionRequest
    {
        public int? AccountId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
    }
}