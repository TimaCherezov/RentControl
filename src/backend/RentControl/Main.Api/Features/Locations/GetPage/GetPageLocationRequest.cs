namespace Main.Api.Features.Locations.GetPage;

public sealed class GetPageLocationRequest
{
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 20;
}
