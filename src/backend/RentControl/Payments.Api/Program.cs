using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Payments.Api;
using Payments.Api.Features.Invoices;
using Payments.Api.Features.Invoices.FakePay;
using Payments.Api.Features.Invoices.Pay;
using Payments.Model;
using RentControl.ServiceDefaults;
using RentControl.ServiceDefaults.Exceptions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddJwtAuthorizationFromJwks(builder.Configuration);

builder.Services.AddMapster();

builder.AddPaymentsDatabase("paymentsdb");

builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumer<PayInvoiceConsumer>();
    cfg.AddConsumer<FakePayInvoiceConsumer>();
});

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<OrderCreatedConsumer>();

    cfg.AddEntityFrameworkOutbox<PaymentsDbContext>(outbox =>
    {
        outbox.UsePostgres();
        outbox.QueryDelay = TimeSpan.FromSeconds(1);
        outbox.UseBusOutbox();
    });

    cfg.AddConfigureEndpointsCallback((context, name, endpoint) =>
    {
        endpoint.UseEntityFrameworkOutbox<PaymentsDbContext>(context);
    });

    cfg.UsingRabbitMq((context, rmq) =>
    {
        rmq.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        rmq.ConfigureEndpoints(context);
    });
});

builder.Services.AddHttpClient("main-api", client =>
    client.BaseAddress = new Uri("https+http://main-api"));

builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddApiExceptionHandler(exceptions =>
{
    exceptions.Unwrap<RequestException>();
    exceptions.Map<DbUpdateException>(ex =>
        ex.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.UniqueViolation
        }
            ? new ApiErrorResponse(
                StatusCodes.Status409Conflict,
                [new ApiError(string.Empty, "Resource cannot be modified or deleted because it is referenced or conflicts with existing data.")])
            : null);
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // В dev накатываем миграции paymentsdb (симметрично localMigrations в Main.Api).
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Точка входа для WebApplicationFactory в интеграционных тестах.
public partial class Program
{
}
