using Main.Api.Features.Auth.CurrentUser;
using Main.Api.Features.Auth.Login;
using Main.Api.Features.Auth.Logout;
using Main.Api.Features.Auth.Refresh;
using Main.Api.Features.Auth.Register;
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

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRegisterUserService, RegisterUserService>();
            services.AddScoped<ILoginUserService, LoginUserService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ILogoutService, LogoutService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            return services;
        }
    }
}
