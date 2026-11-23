using Main.Model.Models;
using Microsoft.AspNetCore.Identity;
using RentControl.ServiceDefaults;

namespace Main.Api.Features.Auth;

public static class AuthServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAuth(IConfiguration configuration)
        {
            services.AddJwtSigning(configuration);

            services.AddHttpContextAccessor();

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            return services;
        }
    }
}
