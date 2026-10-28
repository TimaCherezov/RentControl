using Main.Model.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RentControl.Common.UnitOfWork;

namespace Main.Model;

public static class MainModelExtensions
{
    public static IHostApplicationBuilder AddMainDatabase(
        this IHostApplicationBuilder builder,
        string connectionName = "maindb")
    {
        builder.AddNpgsqlDbContext<MainDbContext>(connectionName);
        builder.Services.AddDbContextFactory<MainDbContext>();

        builder.Services.AddSingleton<IUnitOfWorkFactory<IMainUnitOfWork>, MainUnitOfWorkFactory>();

        return builder;
    }
}
