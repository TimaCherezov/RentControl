namespace Main.Api.Features.ProductImages.Update;

public record UpdateProductImageRequest(
    Guid ProductId,
    string StorageKey,
    bool IsMain,
    string? AltText
);
