using RentControl.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var requested = (builder.Configuration["Services"] ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var unknown = requested.Except(ServiceNames.All).ToArray();
if (unknown.Length > 0)
{
    throw new InvalidOperationException(
        $"Неизвестные сервисы в Services: {string.Join(", ", unknown)}. Доступные: {string.Join(", ", ServiceNames.All)}");
}

var enabled = requested.Length == 0 ? ServiceNames.All : requested;

// Фронт ходит в оба API — они должны подниматься вместе с ним.
if (enabled.Contains(ServiceNames.Front))
{
    enabled = enabled.Union([ServiceNames.MainApi, ServiceNames.PaymentsApi]).ToArray();
}

bool IsEnabled(string name) => enabled.Contains(name);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume()
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<ProjectResource>? mainApi = null;
IResourceBuilder<ProjectResource>? paymentsApi = null;

if (IsEnabled(ServiceNames.MainApi) || IsEnabled(ServiceNames.MainWorker))
{
    var mainDb = postgres.AddDatabase("maindb");

    if (IsEnabled(ServiceNames.MainApi))
    {
        mainApi = builder.AddProject<Projects.Main_Api>(ServiceNames.MainApi)
            .WithReference(mainDb).WaitFor(mainDb)
            .WithReference(rabbitmq).WaitFor(rabbitmq);
    }

    if (IsEnabled(ServiceNames.MainWorker))
    {
        builder.AddProject<Projects.Main_Worker>(ServiceNames.MainWorker)
            .WithReference(mainDb).WaitFor(mainDb)
            .WithReference(rabbitmq).WaitFor(rabbitmq);
    }
}

if (IsEnabled(ServiceNames.PaymentsApi))
{
    var paymentsDb = postgres.AddDatabase("paymentsdb");

    paymentsApi = builder.AddProject<Projects.Payments_Api>(ServiceNames.PaymentsApi)
        .WithReference(paymentsDb).WaitFor(paymentsDb)
        .WithReference(rabbitmq).WaitFor(rabbitmq);

    if (mainApi is not null)
    {
        paymentsApi.WithReference(mainApi);
    }
}

if (IsEnabled(ServiceNames.RealtimeApi))
{
    builder.AddProject<Projects.Realtime_Api>(ServiceNames.RealtimeApi)
        .WithReference(rabbitmq).WaitFor(rabbitmq);
}

if (IsEnabled(ServiceNames.Front))
{
    builder.AddViteApp(ServiceNames.Front, "../../../frontend/RentControl")
        .WithExternalHttpEndpoints()
        .WithReference(mainApi!).WaitFor(mainApi!)
        .WithReference(paymentsApi!).WaitFor(paymentsApi!);
}

builder.Build().Run();
