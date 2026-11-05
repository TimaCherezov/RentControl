namespace Main.Api.Features.ProductImages.Update;

public record UpdateProductImageCommand(
    Guid Id,
    Guid ProductId,
    string StorageKey,
    bool IsMain,
    string? AltText
);
