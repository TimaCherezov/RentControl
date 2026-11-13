using Main.Api.Features.Products;
using Main.Api.Features.Products.Create;
using Main.Api.Features.Products.GetMaxAvailableDuration;
using Main.Api.Features.Products.GetPage;
using Main.Api.Features.Products.Update;
using Main.Api.Storage;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class ProductMapConfig
{
    public static ProductDto ToDto(this Product p, ImageStorageOptions imageStorageOptions, decimal depositRate) =>
        new ProductDto(
            p.Id,
            p.Name,
            p.MarketValue,
            p.DepositOverride,
            p.CalculateDeposit(depositRate),
            p.DailyRate,
            p.HourlyRate,
            p.WeeklyRate,
            p.MonthlyRate,
            p.Description,
            Images: p.Images.ToDto(imageStorageOptions)
        );

    public static List<ProductDto> ToDto(this IEnumerable<Product> b, ImageStorageOptions imageStorageOptions, decimal depositRate) =>
        b.Select(b => b.ToDto(imageStorageOptions, depositRate)).ToList();

    public static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<CreateProductRequest, CreateProductCommand>();

        config.NewConfig<UpdateProductRequest, UpdateProductCommand>();

        config.NewConfig<CreateProductCommand, Product>();

        config.NewConfig<UpdateProductCommand, Product>().Ignore(dest => dest.Id);

        config.NewConfig<GetPageProductRequest, GetPageProductCommand>();

        config.NewConfig<GetMaxAvailableDurationRequest, GetMaxAvailableDurationCommand>();
    }
}
