namespace Main.Api.Features.ProductImages;

public sealed class ImageStorageOptions
{
    public const string Section = "Storage";

    public string ImageBaseUrl { get; set; } = null!;

    public string LocalPath { get; set; } = "uploads";

    public string RequestPath { get; set; } = "/images";

    public string Build(string storageKey) =>
        $"{ImageBaseUrl.TrimEnd('/')}/{storageKey.TrimStart('/')}";
}
