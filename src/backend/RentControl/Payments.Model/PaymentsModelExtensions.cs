using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payments.Model.Repositories;
using Payments.Model.UnitOfWork;
using RentControl.Common.UnitOfWork;

namespace Payments.Model;

public static class PaymentsModelExtensions
{
    public static IHostApplicationBuilder AddPaymentsDatabase(
        this IHostApplicationBuilder builder,
        string connectionName = "paymentsdb")
    {
        // DisableRetry: retry-стратегия Npgsql запрещает явные BeginTransaction извне,
        // а Consumer Outbox MassTransit открывает собственные транзакции на консьюмерах.
        builder.AddNpgsqlDbContext<PaymentsDbContext>(connectionName,
            configureSettings: settings => settings.DisableRetry = true);
        builder.Services.AddDbContextFactory<PaymentsDbContext>();

        builder.Services.AddSingleton<IUnitOfWorkFactory<IPaymentsUnitOfWork>, PaymentsUnitOfWorkFactory>();
        builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        return builder;
    }
}
