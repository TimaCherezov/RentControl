namespace Main.Api.Features.Products.GetPage;

public sealed class GetPageProductRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
