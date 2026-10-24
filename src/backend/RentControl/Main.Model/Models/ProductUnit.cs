namespace Main.Model.Models;

public class ProductUnit
{
    public Guid Id { get; set; }
    public required Product Product { get; set; }
    public Guid ProductId { get; set; }
    public required State State { get; set; }
    public Guid StateId { get; set; }
    public required string SerialNumber { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public required Location Location { get; set; }
    public Guid LocationId { get; set; }
}
