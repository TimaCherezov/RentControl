namespace Main.Api.Features.ProductUnits.GetAvailable;

public record GetAvailableProductUnitsCommand(Guid ProductId, DateTimeOffset Start, DateTimeOffset End);