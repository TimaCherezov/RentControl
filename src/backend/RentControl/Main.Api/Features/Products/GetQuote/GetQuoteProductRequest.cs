namespace Main.Api.Features.Products.GetQuote;

public sealed class GetQuoteProductRequest
{
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
}