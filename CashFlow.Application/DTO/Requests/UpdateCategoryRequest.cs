public class UpdateCategoryRequest
{
    public required int CategoryId { get; set; }
    public required string NewName { get; set; }
    public required string NewColor { get; set; }
    public required string NewIcon { get; set; }
    public required string NewType { get; set; } 
}