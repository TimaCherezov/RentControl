using Main.Api.Mappers;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Main.Api;

public static class MapsterServiceCollectionExtensions
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        ProductMapConfig.Configure(TypeAdapterConfig.GlobalSettings);

        return services;
    }
}
