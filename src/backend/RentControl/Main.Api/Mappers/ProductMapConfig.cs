using Main.Api.Features.Products;
using Main.Api.Features.Products.Create;
using Main.Api.Features.Products.Update;
using Main.Model.Models;
using Mapster;
using Main.Api.Storage;

namespace Main.Api.Mappers;

public static class ProductMapConfig
{
    public static ProductDto ToDto(
        this Product p,
        ImageStorageOptions opt
    ) =>
        new ProductDto(
            p.Id,
            p.Name,
            p.MarketValue,
            p.DepositOverride,
            p.DailyRate,
            p.HourlyRate,
            p.WeeklyRate,
            p.MonthlyRate,
            p.Description,
            Images: p.Images.ToDto(opt)
        );

    public static List<ProductDto> ToDto(this IEnumerable<Product> b, ImageStorageOptions opt) =>
        b.Select(b => b.ToDto(opt)).ToList();

	public static void Configure(TypeAdapterConfig config)
	{
		config.NewConfig<CreateProductRequest, CreateProductCommand>()
			.ConstructUsing(src => new CreateProductCommand(
				src.Name, src.MarketValue, src.DepositOverride,
				src.DailyRate, src.HourlyRate, src.WeeklyRate, src.MonthlyRate, src.Description));

		config.NewConfig<UpdateProductRequest, UpdateProductCommand>()
			.ConstructUsing(src => new UpdateProductCommand(
				Guid.Empty, src.Name!, src.MarketValue, src.DepositOverride,
				src.DailyRate, src.HourlyRate, src.WeeklyRate, src.MonthlyRate, src.Description));

		config.NewConfig<CreateProductCommand, Product>()
			.Ignore(dest => dest.Id)
			.Ignore(dest => dest.Images);

		config.NewConfig<UpdateProductCommand, Product>()
			.Ignore(dest => dest.Id)
			.Ignore(dest => dest.Images);
	}
}
