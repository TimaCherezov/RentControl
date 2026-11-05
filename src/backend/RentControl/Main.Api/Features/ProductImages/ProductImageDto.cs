namespace Main.Api.Features.ProductImages;

public record ProductImageDto(
    Guid Id,
    Guid ProductId,
    string StorageKey,
    bool IsMain,
    DateTimeOffset CreatedAt,
    string? AltText
);
