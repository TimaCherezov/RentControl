namespace Main.Model.Models;

public class BookingStatus
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}
