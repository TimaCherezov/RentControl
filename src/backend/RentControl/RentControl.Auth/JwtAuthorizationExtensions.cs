using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace RentControl.Auth;

public static class JwtAuthorizationExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddJwtAuthorization(
            IConfiguration configuration)
        {
            var settings = configuration
                .GetSection(JwtSettings.Section)
                .Get<JwtSettings>()
                ?? throw new InvalidOperationException(
                    $"Configuration section '{JwtSettings.Section}' is missing.");

            if (string.IsNullOrWhiteSpace(settings.SecretKey))
            {
                throw new InvalidOperationException(
                    $"'{JwtSettings.Section}:SecretKey' is not configured.");
            }

            services.Configure<JwtSettings>(
                configuration.GetSection(JwtSettings.Section));

            services.AddAuthentication(
                    JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(settings.SecretKey)),
                            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                            ValidateIssuer = !string.IsNullOrEmpty(settings.Issuer),
                            ValidIssuer = settings.Issuer,
                            ValidateAudience = !string.IsNullOrEmpty(settings.Audience),
                            ValidAudience = settings.Audience,
                            ValidateLifetime = true,
                            RequireExpirationTime = true
                        };
                });

            services.AddAuthorization();

            return services;
        }
    }
}