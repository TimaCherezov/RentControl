namespace Main.Model.Models;

public class Location
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
}
