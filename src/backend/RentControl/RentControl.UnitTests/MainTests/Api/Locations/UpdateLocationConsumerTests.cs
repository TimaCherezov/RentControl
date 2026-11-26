using Main.Api.Exceptions;
using Main.Api.Features.Locations.Update;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Locations;

public class UpdateLocationConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<ILocationRepository> _locations = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public UpdateLocationConsumerTests()
    {
        _uow.SetupGet(u => u.Locations).Returns(_locations.Object);
        _factory.RunsWith(_uow);
    }

    [Fact]
    public async Task Consume_WhenLocationNotFound_ThrowsObjectNotFoundException()
    {
        var locationId = Guid.NewGuid();
        _locations
            .Setup(l => l.GetByIdAsync(locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new UpdateLocationCommand(locationId, "Renamed", "New Address"));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenLocationExists_AppliesCommandToLocation()
    {
        var location = new LocationBuilder().WithName("Old name").WithAddress("Old address").Build();
        _locations
            .Setup(l => l.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new UpdateLocationCommand(location.Id, "Renamed", "New Address"));

        await consumer.Consume(context.Object);

        Assert.Equal("Renamed", location.Name);
        Assert.Equal("New Address", location.Address);
    }

    private UpdateLocationConsumer CreateConsumer() => new(_factory.Object);
}
