using Main.Api.Mappers;
using Mapster;

namespace Main.Api;

public static class MapsterServiceCollectionExtensions
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        ProductMapConfig.Configure(TypeAdapterConfig.GlobalSettings);
        BookingMapConfig.Configure(TypeAdapterConfig.GlobalSettings);
        LocationMapConfig.Configure(TypeAdapterConfig.GlobalSettings);
        ProductUnitMapConfig.Configure(TypeAdapterConfig.GlobalSettings);

        return services;
    }
}
