using Mapster;
using Payments.Api.Mappers;

namespace Payments.Api;

public static class MapsterServiceCollectionExtensions
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        OrderCreatedMapConfig.Configure(TypeAdapterConfig.GlobalSettings);

        return services;
    }
}
