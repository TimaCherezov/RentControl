namespace Main.Api.Features.ProductImages.Create;

public record CreateProductImageCommand(Guid ProductId, string StorageKey, string? AltText);
