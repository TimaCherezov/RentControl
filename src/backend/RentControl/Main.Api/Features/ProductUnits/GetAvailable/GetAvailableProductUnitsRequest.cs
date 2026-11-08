namespace Main.Api.Features.ProductUnits.GetAvailable;

public sealed class GetAvailableProductUnitsRequest
{
    public Guid ProductId { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
