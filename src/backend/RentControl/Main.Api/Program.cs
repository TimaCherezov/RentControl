using FluentValidation;
using FluentValidation.AspNetCore;
using Main.Api.Auth;
using MassTransit;
using Main.Api.Auth.Register;
using Main.Api.Features.Products.Create;
using Main.Api.Features.Products.Patch;
using Main.Api.Features.Products.Update;
using Main.Api.Features.Validation;
using Main.Api.Middleware;
using Main.Model;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddMainDatabase("maindb");

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddMediator(cfg =>
{
    // cfg.AddConsumers(typeof(Program).Assembly);
    cfg.AddConsumer<CreateProductConsumer>();
    cfg.AddConsumer<PatchProductConsumer>();
    cfg.AddConsumer<UpdateProductConsumer>();
    
    cfg.ConfigureMediator((context, mediatorCfg) =>
    {
        mediatorCfg.UseSendFilter(typeof(CommandValidationFilter<>), context);
    });
});

var app = builder.Build();

// Раскомментируйте, если миграции не подтягиваются автоматически при запуске
// #region localMigrations
// await using (var scope = app.Services.CreateAsyncScope())
// {
//     var dbContext = scope.ServiceProvider
//         .GetRequiredService<MainDbContext>();
//
//     await dbContext.Database.MigrateAsync();
// }
// #endregion

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
