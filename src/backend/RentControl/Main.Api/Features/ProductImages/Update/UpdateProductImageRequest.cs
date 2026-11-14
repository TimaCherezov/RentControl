namespace Main.Api.Features.ProductImages.Update;

public record UpdateProductImageRequest(string StorageKey, bool IsMain, string? AltText);
