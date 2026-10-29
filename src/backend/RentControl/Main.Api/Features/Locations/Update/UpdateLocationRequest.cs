namespace Main.Api.Features.Locations.Update;

public sealed class UpdateLocationRequest
{
	public string? Name { get; init; }
	public string? Address { get; init; }
}