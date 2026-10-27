namespace Main.Model.Models;

public class ProductImage
{
    public Guid Id { get; set; }
    public Product Product { get; set; } = null!;
    public Guid ProductId { get; set; }
    public string StorageKey { get; set; } = null!;
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public string? AltText { get; set; }
}
