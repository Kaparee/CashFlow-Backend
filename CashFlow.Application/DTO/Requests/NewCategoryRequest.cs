namespace CashFlow.Application.DTO.Requests
{
    public class NewCategoryRequest
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public string? Type { get; set; }
        public decimal? LimitAmount { get; set; }
    }
}