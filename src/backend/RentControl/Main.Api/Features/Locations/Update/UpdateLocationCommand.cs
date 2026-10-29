namespace Main.Api.Features.Locations.Update;

public record UpdateLocationCommand(
    Guid Id,
    string Name,
    string Address
);
