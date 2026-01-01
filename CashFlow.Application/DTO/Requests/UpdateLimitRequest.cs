public class UpdateLimitRequest
{
    public required int LimitId { get; set; }
    public required int NewCategoryId { get; set; }
    public required string NewName { get; set; }
    public required decimal NewValue { get; set; }
    public required string NewDescription { get; set; }
    public required DateTime NewStartDate { get; set; }
    public required DateTime NewEndDate { get; set; }
}