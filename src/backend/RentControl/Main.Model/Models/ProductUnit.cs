namespace Main.Model.Models;

/// <summary>
/// Конкретная единица продукта, имеющая идентификационный (серийный) номер.
/// Продукт считается доступным для бронирования, если есть как минимум одна единица продукта.
/// </summary>
public class ProductUnit
{
    public Guid Id { get; set; }
    public Product Product { get; set; } = null!;
    public Guid ProductId { get; set; }
    public StateEnum State { get; set; }
    public string SerialNumber { get; set; } = null!;

    /// <summary>
    /// Дата приобретения продукта.
    /// </summary>
    public DateOnly PurchaseDate { get; set; }

    /// <summary>
    /// Стоимость продукта на момент его приобретения.
    /// </summary>
    public decimal PurchasePrice { get; set; }
    public Location Location { get; set; } = null!;
    public Guid LocationId { get; set; }
}
