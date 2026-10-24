using Main.Model;
using Main.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddMainDatabase("maindb");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();