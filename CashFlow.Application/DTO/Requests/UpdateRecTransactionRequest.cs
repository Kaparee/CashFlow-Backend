public class UpdateRecTransactionRequest
{
    public required int RecTransactionId { get; set; }
    public required int AccountId { get; set; }
    public required string NewType { get; set; }
    public required string NewName { get; set; }
    public required string NewFrequency { get; set; }
    public required bool NewIsTrue { get; set; }
    public DateTime? NewEndDate { get; set; }
    public required int NewCategoryId { get; set; }
    public required decimal NewAmount { get; set; }
    public required string NewDescription { get; set; }
    public required DateTime NewNextPaymentDate { get; set; }
}