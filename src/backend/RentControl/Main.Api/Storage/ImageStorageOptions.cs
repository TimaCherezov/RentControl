namespace Main.Api.Storage;

public sealed class ImageStorageOptions
{
    public string BaseUrl { get; set; } = null!;

    public string Build(string storageKey) => $"{BaseUrl.TrimEnd('/')}/{storageKey.TrimStart('/')}";
}
