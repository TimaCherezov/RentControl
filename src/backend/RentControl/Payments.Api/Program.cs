using MassTransit;
using Payments.Api;
using Payments.Api.Features.Invoices;
using Payments.Model;

using RentControl.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddJwtAuthorizationFromJwks(builder.Configuration);

builder.Services.AddMapster();

builder.AddPaymentsDatabase("paymentsdb");

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<OrderCreatedConsumer>();

    cfg.AddEntityFrameworkOutbox<PaymentsDbContext>(outbox =>
    {
        outbox.UsePostgres();
        outbox.QueryDelay = TimeSpan.FromSeconds(1);
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

var app = builder.Build();

app.MapDefaultEndpoints();

// В dev создаём схему paymentsdb по модели (сами миграций пока нет — см. TODO в тестовой фикстуре).
if (app.Environment.IsDevelopment())
{
    await using (var scope = app.Services.CreateAsyncScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .RequireAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Точка входа для WebApplicationFactory в интеграционных тестах.
public partial class Program
{
}
