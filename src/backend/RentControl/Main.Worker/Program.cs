using Main.Model;
using Main.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddMainDatabase("maindb", disableRetry: true);

builder.AddMainWorkerBus();

var host = builder.Build();
host.Run();

/// <summary>Точка входа для хостинга в тестах.</summary>
public partial class Program;
