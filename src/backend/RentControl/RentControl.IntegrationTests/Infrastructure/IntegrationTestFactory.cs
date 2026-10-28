using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RentControl.IntegrationTests.Infrastructure;

/// <summary>
/// Фабрика тестируемого сервиса в памяти: приложение целиком поднимается
/// через WebApplicationFactory, внешние зависимости (БД, брокер) задаются
/// конфигурацией — их адреса приходят из Testcontainers-фикстур.
/// </summary>
/// <remarks>
/// Переопределения конфигурации передаются через UseSetting и попадают в приложение
/// аргументами командной строки (высший приоритет поверх appsettings). Это работает
/// с минимальным хостингом (WebApplication.CreateBuilder), в отличие от
/// ConfigureAppConfiguration, который до конфигурации приложения не доходит.
/// Ключи должны быть плоскими, например "ConnectionStrings:maindb".
/// </remarks>
public sealed class IntegrationTestFactory<TProgram>(
    IReadOnlyDictionary<string, string?>? configuration = null,
    Action<IServiceCollection>? configureServices = null)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);

        if (configuration is not null)
        {
            foreach (var (key, value) in configuration)
            {
                builder.UseSetting(key, value);
            }
        }

        if (configureServices is not null)
        {
            builder.ConfigureServices(configureServices);
        }
    }
}
