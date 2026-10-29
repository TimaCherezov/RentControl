using RentControl.IntegrationTests.Main.Infrastructure;
using RentControl.IntegrationTests.Payments.Infrastructure;
using RentControl.IntegrationTests.Realtime.Infrastructure;

namespace RentControl.IntegrationTests;

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
