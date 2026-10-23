using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payments.Model.UnitOfWork;
using RentControl.Common.UnitOfWork;

namespace Payments.Model;

public static class PaymentsModelExtensions
{
    public static IHostApplicationBuilder AddPaymentsDatabase(
        this IHostApplicationBuilder builder,
        string connectionName = "paymentsdb")
    {
        builder.AddNpgsqlDbContext<PaymentsDbContext>(connectionName);
        builder.Services.AddDbContextFactory<PaymentsDbContext>();

        builder.Services.AddSingleton<IUnitOfWorkFactory<IPaymentsUnitOfWork>, PaymentsUnitOfWorkFactory>();

        return builder;
    }
}
