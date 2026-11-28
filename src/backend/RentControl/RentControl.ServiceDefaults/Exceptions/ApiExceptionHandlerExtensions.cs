using Microsoft.Extensions.DependencyInjection;

namespace RentControl.ServiceDefaults.Exceptions;

public static class ApiExceptionHandlerExtensions
{
    public static IServiceCollection AddApiExceptionHandler(
        this IServiceCollection services,
        Action<ApiExceptionHandlerBuilder>? configure = null)
    {
        var builder = new ApiExceptionHandlerBuilder();
        configure?.Invoke(builder);

        services.AddSingleton(builder);
        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
