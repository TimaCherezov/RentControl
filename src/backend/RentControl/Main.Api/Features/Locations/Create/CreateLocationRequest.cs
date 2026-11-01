namespace Main.Api.Features.Locations.Create;

public record CreateLocationRequest(
	string? Name,
	string? Address);