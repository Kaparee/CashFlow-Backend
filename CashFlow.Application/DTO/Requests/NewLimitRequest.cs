namespace CashFlow.Application.DTO.Requests
{
    public class NewLimitRequest
    {
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public decimal? Value { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public required int AccountId { get; set; }
    }
}