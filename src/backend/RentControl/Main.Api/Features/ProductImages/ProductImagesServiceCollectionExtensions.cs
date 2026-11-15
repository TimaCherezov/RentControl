namespace Main.Api.Features.ProductImages;

public static class ProductImagesServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddProductImages(IConfiguration configuration)
        {
            services
                .AddOptions<ImageStorageOptions>()
                .Bind(configuration.GetSection(ImageStorageOptions.Section))
                .Validate(
                    x => !string.IsNullOrWhiteSpace(x.ImageBaseUrl),
                    "Storage:ImageBaseUrl is required"
                )
                .Validate(
                    x => (!string.IsNullOrWhiteSpace(x.LocalPath)),
                    "Storage:LocalPath is required"
                )
                .Validate(
                    x => !string.IsNullOrWhiteSpace(x.RequestPath) && x.RequestPath.StartsWith('/'),
                    "Storage:RequestPath must be a non-empty absolute path"
                )
                .ValidateOnStart();

            return services;
        }
    }
}
