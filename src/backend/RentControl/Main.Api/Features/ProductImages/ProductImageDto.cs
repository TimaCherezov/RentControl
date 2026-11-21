namespace Main.Api.Features.ProductImages;

public record ProductImageDto(
    Guid Id,
    Guid ProductId,
    bool IsMain,
    DateTimeOffset CreatedAt,
    string? AltText,
    string Url
);
