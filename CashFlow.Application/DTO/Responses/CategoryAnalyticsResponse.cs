namespace CashFlow.Application.DTO.Responses
{
    public class CategoryAnalyticsResponse
    {
        public required int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public required string Color { get; set; }
        public required decimal TotalValue { get; set; }
        public required decimal Percentage { get; set; }
    }
}
      