using Main.Api.Features.ProductUnits;
using Main.Api.Features.ProductUnits.Create;
using Main.Api.Features.ProductUnits.GetAvailable;
using Main.Api.Features.ProductUnits.Update;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class ProductUnitMapConfig
{
    public static void Configure(TypeAdapterConfig config)
    {
        config
            .NewConfig<ProductUnit, ProductUnitDto>()
            .ConstructUsing(src => new ProductUnitDto(
                src.Id,
                src.ProductId,
                src.State,
                src.SerialNumber,
                src.PurchaseDate,
                src.PurchasePrice,
                src.LocationId
            ));

        config
            .NewConfig<CreateProductUnitRequest, CreateProductUnitCommand>()
            .ConstructUsing(src => new CreateProductUnitCommand(
                src.ProductId,
                src.State,
                src.SerialNumber,
                src.PurchaseDate,
                src.PurchasePrice,
                src.LocationId
            ));

        config
            .NewConfig<UpdateProductUnitRequest, UpdateProductUnitCommand>()
            .ConstructUsing(src => new UpdateProductUnitCommand(
                Guid.Empty,
                src.ProductId,
                src.State,
                src.SerialNumber!,
                src.PurchaseDate,
                src.PurchasePrice,
                src.LocationId
            ));

        config.NewConfig<GetAvailableProductUnitsRequest, GetAvailableProductUnitsCommand>();

        config
            .NewConfig<CreateProductUnitCommand, ProductUnit>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Product)
            .Ignore(dest => dest.Location);

        config
            .NewConfig<UpdateProductUnitCommand, ProductUnit>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Product)
            .Ignore(dest => dest.Location);
    }
}
