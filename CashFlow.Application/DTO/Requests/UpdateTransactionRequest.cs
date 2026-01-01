public class UpdateTransactionRequest
{
    public required int TransactionId { get; set; }
    public required int AccountId { get; set; }
    public required int NewCategoryId { get; set; }
    public required decimal NewAmount { get; set; }
    public required string NewDescription { get; set; }
    public required string NewType { get; set; }
    public required DateTime NewDate { get; set; }
}