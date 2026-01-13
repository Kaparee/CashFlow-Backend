namespace CashFlow.Application.DTO.Requests
{
    public class NewRecTransactionRequest
    {
        public required string Type { get; set; }
        public required string Name { get; set; }
        public required string Frequency { get; set; }
        public required bool isTrue { get; set; }
        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? AccountId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Amount { get; set; }
        public required string Description { get; set; }
        public required DateTime NextPaymentDate { get; set; }
    }
}