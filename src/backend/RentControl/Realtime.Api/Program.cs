using MassTransit;
using Realtime.Api.Consumers;
using Realtime.Api.Hubs;
using RentControl.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddJwtAuthorizationFromJwks(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderChangedConsumer>();

    configurator.UsingRabbitMq((context, rabbitMq) =>
    {
        rabbitMq.Host(
            builder.Configuration.GetConnectionString("rabbitmq"));
        rabbitMq.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<BookingHub>("/hubs/bookings")
    .RequireAuthorization();

app.Run();

public partial class Program;
