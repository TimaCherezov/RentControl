using Main.Api.Features.Products;
using Main.Api.Features.Products.Create;
using Main.Api.Features.Products.Update;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class ProductMapConfig
{
	public static void Configure(TypeAdapterConfig config)
	{
		config.NewConfig<Product, ProductDto>()
			.ConstructUsing(src => new ProductDto(
				src.Id, src.Name, src.MarketValue, src.DepositOverride,
				src.DailyRate, src.HourlyRate, src.WeeklyRate, src.MonthlyRate, src.Description));

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
