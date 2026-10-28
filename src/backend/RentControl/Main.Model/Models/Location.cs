namespace Main.Model.Models;

/// <summary>
/// Место размещения пункта, который занимается выдачей и приемом продуктов.
/// </summary>
public class Location
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
}
