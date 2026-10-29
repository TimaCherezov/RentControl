namespace Main.Api.Features.Locations.Create;

public record CreateLocationCommand(
    string Name,
    string Address
);