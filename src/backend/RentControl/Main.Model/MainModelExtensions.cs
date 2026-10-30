using Main.Model.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EFCore.PostgresExtensions.Extensions;
using RentControl.Common.UnitOfWork;

namespace Main.Model;

public static class MainModelExtensions
{
    public static IHostApplicationBuilder AddMainDatabase(
        this IHostApplicationBuilder builder,
        string connectionName = "maindb")
    {
        builder.AddNpgsqlDbContext<MainDbContext>(connectionName, configureDbContextOptions: opt => opt.UseQueryLocks());
        builder.Services.AddDbContextFactory<MainDbContext>();

        builder.Services.AddSingleton<IUnitOfWorkFactory<IMainUnitOfWork>, MainUnitOfWorkFactory>();

        return builder;
    }
}
