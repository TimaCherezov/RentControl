using Main.Api.Auth.CurrentUser;
using Main.Api.Auth.Login;
using Main.Api.Auth.Logout;
using Main.Api.Auth.Refresh;
using Main.Api.Auth.Register;
using Main.Model.Models;
using Microsoft.AspNetCore.Identity;
using RentControl.Auth;

namespace Main.Api.Auth;

public static class AuthServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAuth(IConfiguration configuration)
        {
            services.AddJwtAuthorization(configuration);

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
