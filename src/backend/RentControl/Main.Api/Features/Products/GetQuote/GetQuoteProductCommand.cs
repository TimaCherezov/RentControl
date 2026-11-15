namespace Main.Api.Features.Products.GetQuote;

public record GetQuoteProductCommand(
    Guid Id,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt);