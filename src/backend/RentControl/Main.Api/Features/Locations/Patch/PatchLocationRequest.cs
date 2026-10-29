namespace Main.Api.Features.Locations.Patch;

public sealed class PatchLocationRequest
{
	public string? Name { get; init; }
	public string? Address { get; init; }
}