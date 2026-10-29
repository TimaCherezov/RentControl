namespace Main.Api.Features.Locations.Patch;

public record PatchLocationCommand(
    Guid Id,
    string? Name,
    string? Address
);