using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Realtime.Api.Consumers;
using Realtime.Api.Hubs;
using Realtime.Model;
using RentControl.Common.IntegrationEvents;

namespace RentControl.UnitTests.RealtimeTests.Api.Consumers;

public class OrderChangedConsumerTests
{
    private static readonly Guid BookingId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private const string Status = "ReadyForPickup";

    private string? _sentToGroup;
    private string? _sentMethod;
    private object?[]? _sentArgs;

    private readonly Mock<IHubContext<BookingHub>> _hubContext = new();
    private readonly Mock<IHubClients> _clients = new();
    private readonly Mock<IClientProxy> _clientProxy = new();

    public OrderChangedConsumerTests()
    {
        _hubContext.SetupGet(h => h.Clients).Returns(_clients.Object);
        _clients
            .Setup(c => c.Group(It.IsAny<string>()))
            .Callback<string>(group => _sentToGroup = group)
            .Returns(_clientProxy.Object);
        _clientProxy
            .Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Callback<string, object?[], CancellationToken>((method, args, _) =>
            {
                _sentMethod = method;
                _sentArgs = args;
            })
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Consume_SendsToUserGroupOfMessageUser()
    {
        var consumer = CreateConsumer();
        var context = CreateContext();

        await consumer.Consume(context.Object);

        Assert.Equal($"user:{UserId}", _sentToGroup);
        _clientProxy.Verify(
            p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_SendsBookingChangedEventWithBookingIdAndStatus()
    {
        var consumer = CreateConsumer();
        var context = CreateContext();

        await consumer.Consume(context.Object);

        Assert.Equal("BookingChanged", _sentMethod);
        var notification = Assert.IsType<BookingChangedNotification>(_sentArgs?[0]);
        Assert.Equal(BookingId, notification.BookingId);
        Assert.Equal(Status, notification.Status);
        _clientProxy.Verify(
            p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private OrderChangedConsumer CreateConsumer() =>
        new(_hubContext.Object, Mock.Of<ILogger<OrderChangedConsumer>>());

    private static Mock<ConsumeContext<OrderChanged>> CreateContext() =>
        TestConsumeContext.For(new OrderChanged(BookingId, UserId, Status));
}
