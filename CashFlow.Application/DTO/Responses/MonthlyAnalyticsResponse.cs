namespace CashFlow.Application.DTO.Responses
{
    public class MonthlyAnalyticsResponse
    {
        public required int MonthNumber { get; set; }
        public required decimal TotalExpenseAmount { get; set; }
        public required decimal TotalIncomeAmount { get; set; }
        public required decimal Balance { get; set; }
    }
}