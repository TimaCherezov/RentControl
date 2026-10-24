namespace Main.Model.Models;

public class ProductImage
{
    public Guid Id { get; set; }
    public required Product Product { get; set; }
    public Guid ProductId { get; set; }
    public required string StorageKey { get; set; }
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public string? AltText { get; set; }
}
