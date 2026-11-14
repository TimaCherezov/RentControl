namespace Main.Api.Features.ProductImages.Update;

public record UpdateProductImageCommand(Guid Id, string StorageKey, bool IsMain, string? AltText);
