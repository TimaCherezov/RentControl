using Main.Api.Features.ProductUnits;
using Main.Api.Features.ProductUnits.Create;
using Main.Api.Features.ProductUnits.Update;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class ProductUnitMapConfig
{
    public static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<ProductUnit, ProductUnitDto>();

        config.NewConfig<CreateProductUnitRequest, CreateProductUnitCommand>();

        config.NewConfig<UpdateProductUnitRequest, UpdateProductUnitCommand>();

        config.NewConfig<CreateProductUnitCommand, ProductUnit>();

        config.NewConfig<UpdateProductUnitCommand, ProductUnit>().Ignore(dest => dest.Id);
    }
}
