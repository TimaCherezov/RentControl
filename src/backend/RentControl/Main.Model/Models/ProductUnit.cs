namespace Main.Model.Models;

public class ProductUnit
{
    public Guid Id { get; set; }
    public Product Product { get; set; } = null!;
    public Guid ProductId { get; set; }
    public State State { get; set; } = null!;
    public Guid StateId { get; set; }
    public string SerialNumber { get; set; } = null!;
    public DateOnly PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public Location Location { get; set; } = null!;
    public Guid LocationId { get; set; }
}
