using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace RentControl.ServiceDefaults;

public static class JwtAuthorizationExtensions
{
    // Имя клиента публичное: интеграционные тесты Payments.Api подменяют его обработчик заглушкой JWKS.
    public const string JwksHttpClientName = "jwt-jwks";
    private const string MainApiUrlConfigurationKey =
        "ServiceDiscovery:MainApi";
    private const string JwksPath = "/.well-known/jwks.json";

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
            var mainApiUrl = configuration[MainApiUrlConfigurationKey];

            if (string.IsNullOrWhiteSpace(mainApiUrl))
            {
                throw new InvalidOperationException(
                    $"'{MainApiUrlConfigurationKey}' is not configured.");
            }

            services.AddHttpClient(
                JwksHttpClientName,
                client => client.BaseAddress = new Uri(mainApiUrl));

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
                            JwksPath,
                            new JwksConfigurationRetriever(),
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
        if (string.IsNullOrWhiteSpace(settings.Issuer))
        {
            throw new InvalidOperationException(
                $"'{JwtOptions.Section}:Issuer' is not configured.");
        }

        if (string.IsNullOrWhiteSpace(settings.Audience))
        {
            throw new InvalidOperationException(
                $"'{JwtOptions.Section}:Audience' is not configured.");
        }

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.Section));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true
                });

        services.AddAuthorization();
    }
}
