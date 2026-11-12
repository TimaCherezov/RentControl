using Main.Api.Features.ProductImages;
using Main.Api.Features.ProductImages.Create;
using Main.Api.Features.ProductImages.Update;
using Main.Api.Storage;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class ProductImageMapConfig
{
    public static ProductImageDto ToDto(this ProductImage pi, ImageStorageOptions opt) =>
        new ProductImageDto(
            pi.Id,
            pi.ProductId,
            pi.StorageKey,
            pi.IsMain,
            pi.CreatedAt,
            pi.AltText,
            Url: opt.Build(pi.StorageKey)
        );

    public static List<ProductImageDto> ToDto(
        this IEnumerable<ProductImage> src,
        ImageStorageOptions opt
    ) => src.Select(i => i.ToDto(opt)).ToList();

    public static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<CreateProductImageRequest, CreateProductImageCommand>();

        config.NewConfig<UpdateProductImageRequest, UpdateProductImageCommand>();

        config.NewConfig<CreateProductImageCommand, ProductImage>();

        config.NewConfig<UpdateProductImageCommand, ProductImage>().Ignore(dest => dest.Id);
    }
}
