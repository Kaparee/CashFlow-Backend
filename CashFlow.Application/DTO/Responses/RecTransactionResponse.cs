namespace CashFlow.Application.DTO.Responses
{
    public class RecTransactionResponse
    {
        public int RecTransactionId { get; set; }
        public string? Name { get; set; }
        public decimal Amount { get; set; }
        public string? Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Type { get; set; }
        public DateTime NextPaymentDate { get; set; }

        public int? AccountId { get; set; }
        public string? AccountName { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}