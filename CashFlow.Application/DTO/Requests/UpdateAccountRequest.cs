public class UpdateAccountRequest
{
    public required int AccountId { get; set; }
    public required string NewName { get; set; }
    public required string NewPhotoUrl { get; set; }
}