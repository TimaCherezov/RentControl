namespace Main.Api.Features.ProductImages.Create;

public record CreateProductImageRequest(Guid ProductId, string StorageKey, string? AltText);
