using Main.Api.Features.Locations;
using Main.Api.Features.Locations.Create;
using Main.Api.Features.Locations.Update;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class LocationMapConfig
{
	public static void Configure(TypeAdapterConfig config)
	{
		config.NewConfig<Location, LocationDto>()
			.ConstructUsing(src => new LocationDto(
				src.Id, src.Name, src.Address));

		config.NewConfig<CreateLocationRequest, CreateLocationCommand>()
			.ConstructUsing(src => new CreateLocationCommand(
				src.Name!, src.Address!));

		config.NewConfig<UpdateLocationRequest, UpdateLocationCommand>()
			.ConstructUsing(src => new UpdateLocationCommand(
				Guid.Empty, src.Name!, src.Address!));

		config.NewConfig<CreateLocationCommand, Location>()
			.Ignore(dest => dest.Id);

		config.NewConfig<UpdateLocationCommand, Location>()
			.Ignore(dest => dest.Id);
	}
}