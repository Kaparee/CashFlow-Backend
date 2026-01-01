public class UpdateKeyWordRequest
{
    public required int KeyWordId { get; set; }
    public required int NewCategoryId { get; set; }
    public required string NewWord { get; set; }
}