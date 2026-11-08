using FluentValidation;
using FluentValidation.AspNetCore;
using Main.Api;
using Main.Api.Auth;
using Main.Api.Auth.Register;
using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.Create;
using Main.Api.Features.Bookings.Get;
using Main.Api.Features.Bookings.GetPage;
using Main.Api.Features.Bookings.Cancel;
using Main.Api.Features.Bookings.Complete;
using Main.Api.Features.Bookings.Confirm;
using Main.Api.Features.Bookings.Issue;
using Main.Api.Features.Bookings.Overdue;
using Main.Api.Features.Bookings.ReadyForPickup;
using Main.Api.Features.Bookings.Return;
using Main.Api.Features.Bookings.Update;
using Main.Api.Features.Locations.Create;
using Main.Api.Features.Locations.Delete;
using Main.Api.Features.Locations.Get;
using Main.Api.Features.Locations.Update;
using Main.Api.Features.ProductImages.Create;
using Main.Api.Features.ProductImages.Delete;
using Main.Api.Features.ProductImages.Get;
using Main.Api.Features.ProductImages.GetAll;
using Main.Api.Features.ProductImages.GetMain;
using Main.Api.Features.ProductImages.Update;
using Main.Api.Features.Products.Create;
using Main.Api.Features.Products.Delete;
using Main.Api.Features.Products.Get;
using Main.Api.Features.Products.GetPage;
using Main.Api.Features.Products.Update;
using Main.Api.Features.ProductUnits.Create;
using Main.Api.Features.ProductUnits.Delete;
using Main.Api.Features.ProductUnits.Get;
using Main.Api.Features.ProductUnits.GetAvailable;
using Main.Api.Features.ProductUnits.Update;
using Main.Api.Features.Validation;
using Main.Api.Middleware;
using Main.Model;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddMainDatabase("maindb");

builder.Services.AddAuth(builder.Configuration);
builder.Services.AddMapster();
builder.Services.AddBookings();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumer<CreateProductConsumer>();
    cfg.AddConsumer<UpdateProductConsumer>();
    cfg.AddConsumer<GetPageProductConsumer>();
    cfg.AddConsumer<GetProductConsumer>();
    cfg.AddConsumer<DeleteProductConsumer>();

    cfg.AddConsumer<CreateBookingConsumer>();
    cfg.AddConsumer<UpdateBookingConsumer>();
    cfg.AddConsumer<GetBookingConsumer>();
    cfg.AddConsumer<GetPageBookingConsumer>();
    cfg.AddConsumer<IssueBookingConsumer>();
    cfg.AddConsumer<ReturnBookingConsumer>();
    cfg.AddConsumer<ConfirmBookingConsumer>();
    cfg.AddConsumer<CancelBookingConsumer>();
    cfg.AddConsumer<ReadyForPickupBookingConsumer>();
    cfg.AddConsumer<OverdueBookingConsumer>();
    cfg.AddConsumer<CompleteBookingConsumer>();

    cfg.AddConsumer<CreateLocationConsumer>();
    cfg.AddConsumer<UpdateLocationConsumer>();
    cfg.AddConsumer<GetLocationConsumer>();
    cfg.AddConsumer<DeleteLocationConsumer>();

    cfg.AddConsumer<CreateProductUnitConsumer>();
    cfg.AddConsumer<GetProductUnitConsumer>();
    cfg.AddConsumer<GetAvailableProductUnitsConsumer>();
    cfg.AddConsumer<UpdateProductUnitConsumer>();
    cfg.AddConsumer<DeleteProductUnitConsumer>();

    cfg.AddConsumer<CreateProductImageConsumer>();
    cfg.AddConsumer<GetProductImageConsumer>();
    cfg.AddConsumer<DeleteProductImageConsumer>();
    cfg.AddConsumer<GetMainProductImageConsumer>();
    cfg.AddConsumer<GetAllProductImageConsumer>();
    cfg.AddConsumer<UpdateProductImageConsumer>();

    cfg.ConfigureMediator(
        (context, mediatorCfg) =>
        {
            mediatorCfg.UseSendFilter(typeof(CommandValidationFilter<>), context);
        }
    );
});

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddEntityFrameworkOutbox<MainDbContext>(outbox =>
    {
        outbox.UsePostgres();
        outbox.QueryDelay = TimeSpan.FromSeconds(1);
        outbox.UseBusOutbox();
    });

    cfg.UsingRabbitMq((_, rmq) =>
    {
        rmq.Host(builder.Configuration.GetConnectionString("rabbitmq"));
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast")
    .RequireAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Точка входа для WebApplicationFactory в интеграционных тестах.
public partial class Program { }
