namespace Main.Api.Features.Locations.Create;

public sealed class CreateLocationRequest
{
	public string? Name { get; init; }
	public string? Address { get; init; }
}