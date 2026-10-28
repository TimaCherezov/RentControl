namespace RentControl.IntegrationTests.Infrastructure;

/// <summary>
/// Образы контейнеров для интеграционных тестов.
/// Совпадают с образами, которые поднимает AppHost.
/// </summary>
internal static class TestImages
{
    public const string Postgres = "postgres:17.6";

    public const string RabbitMq = "rabbitmq:4.2-management";
}
