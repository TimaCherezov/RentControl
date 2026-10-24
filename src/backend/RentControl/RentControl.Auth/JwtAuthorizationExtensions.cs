using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace RentControl.Auth;

public static class JwtAuthorizationExtensions
{
    private const string JwksHttpClientName = "jwt-jwks";

    extension(IServiceCollection services)
    {
        public IServiceCollection AddJwtSigning(
            IConfiguration configuration)
        {
            var settings = GetSettings(configuration);

            services.AddSingleton<JwtSigningKeyProvider>();

            AddCommonAuthentication(services, settings, configuration);

            services
                .AddOptions<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme)
                .PostConfigure<JwtSigningKeyProvider>((options, keyProvider) =>
                {
                    options.TokenValidationParameters.IssuerSigningKey =
                        keyProvider.Key;
                });

            return services;
        }

        public IServiceCollection AddJwtAuthorizationFromJwks(
            IConfiguration configuration)
        {
            var settings = GetSettings(configuration);

            if (string.IsNullOrWhiteSpace(settings.MetadataUrl))
            {
                throw new InvalidOperationException(
                    $"'{JwtOptions.Section}:MetadataUrl' is not configured.");
            }

            services.AddHttpClient(JwksHttpClientName);

            AddCommonAuthentication(services, settings, configuration);

            services
                .AddOptions<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme)
                .Configure<IHttpClientFactory>((options, clientFactory) =>
                {
                    var retriever = new HttpDocumentRetriever(
                        clientFactory.CreateClient(JwksHttpClientName))
                    {
                        RequireHttps = false
                    };

                    options.ConfigurationManager =
                        new ConfigurationManager<OpenIdConnectConfiguration>(
                            settings.MetadataUrl,
                            new OpenIdConnectConfigurationRetriever(),
                            retriever);
                });

            return services;
        }
    }

    private static JwtOptions GetSettings(IConfiguration configuration)
        => configuration.GetSection(JwtOptions.Section).Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{JwtOptions.Section}' is missing.");
    
    private static void AddCommonAuthentication(
        IServiceCollection services,
        JwtOptions settings,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Section));
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                    ValidateIssuer = !string.IsNullOrEmpty(settings.Issuer),
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = !string.IsNullOrEmpty(settings.Audience),
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true
                });

        services.AddAuthorization();
    }
}
