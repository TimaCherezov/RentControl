using Main.Api.Features.Locations;
using Main.Api.Features.Locations.Create;
using Main.Api.Features.Locations.Update;
using Main.Api.Features.Locations.GetPage;
using Main.Model.Models;
using Mapster;

namespace Main.Api.Mappers;

public static class LocationMapConfig
{
    public static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<Location, LocationDto>();

        config.NewConfig<CreateLocationRequest, CreateLocationCommand>();

        config.NewConfig<UpdateLocationRequest, UpdateLocationCommand>();

        config.NewConfig<CreateLocationCommand, Location>();

        config.NewConfig<UpdateLocationCommand, Location>().Ignore(dest => dest.Id);

        config.NewConfig<GetPageLocationRequest, GetPageLocationCommand>();
    }
}
