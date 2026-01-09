namespace CashFlow.Application.DTO.Responses
{
    public class LimitResponse
    {
        public int LimitId { get; set; }
        public decimal Value { get; set; }
        public decimal CurrentAmount { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryIcon { get; set; }
    }
}