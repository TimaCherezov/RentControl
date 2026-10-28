using RentControl.IntegrationTests.Fixtures;
using Xunit;

namespace RentControl.IntegrationTests;

// Коллекции делают фикстуру общей для всех тестовых классов коллекции:
// контейнер Postgres и фабрика приложения стартуют один раз, классы внутри
// коллекции выполняются последовательно, разные коллекции — параллельно.

[CollectionDefinition(MainApiCollection.Name)]
public sealed class MainApiCollection : ICollectionFixture<MainApiFixture>
{
    public const string Name = "MainApi";
}

[CollectionDefinition(PaymentsApiCollection.Name)]
public sealed class PaymentsApiCollection : ICollectionFixture<PaymentsApiFixture>
{
    public const string Name = "PaymentsApi";
}

[CollectionDefinition(RealtimeApiCollection.Name)]
public sealed class RealtimeApiCollection : ICollectionFixture<RealtimeApiFixture>
{
    public const string Name = "RealtimeApi";
}

[CollectionDefinition(RabbitMqCollection.Name)]
public sealed class RabbitMqCollection : ICollectionFixture<RabbitMqFixture>
{
    public const string Name = "RabbitMq";
}
