using Main.Api.Features.ProductImages;
using Main.Api.Features.ProductImages.Create;
using Main.Api.Features.ProductImages.Update;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class ProductImageMapConfig
{
    public static void Configure(TypeAdapterConfig config)
    {
        config
            .NewConfig<ProductImage, ProductImageDto>()
            .ConstructUsing(src => new ProductImageDto(
                src.Id,
                src.ProductId,
                src.StorageKey,
                src.IsMain,
                src.CreatedAt,
                src.AltText
            ));

        config
            .NewConfig<CreateProductImageRequest, CreateProductImageCommand>()
            .ConstructUsing(src => new CreateProductImageCommand(
                src.ProductId,
                src.StorageKey!,
                src.IsMain,
                src.AltText
            ));

        config
            .NewConfig<UpdateProductImageRequest, UpdateProductImageCommand>()
            .ConstructUsing(src => new UpdateProductImageCommand(
                Guid.Empty,
                src.ProductId,
                src.StorageKey!,
                src.IsMain,
                src.AltText
            ));

        config
            .NewConfig<CreateProductImageCommand, ProductImage>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Product);

        config
            .NewConfig<UpdateProductImageCommand, ProductImage>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Product)
            .Ignore(dest => dest.CreatedAt);
    }
}
