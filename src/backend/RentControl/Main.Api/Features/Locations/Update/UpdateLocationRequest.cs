namespace Main.Api.Features.Locations.Update;

public record UpdateLocationRequest(
	string? Name,
	string? Address);