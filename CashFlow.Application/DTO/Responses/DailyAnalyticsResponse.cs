namespace CashFlow.Application.DTO.Responses
{
    public class DailyAnalyticsResponse
    {
        public DateTime Date { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
    }
}