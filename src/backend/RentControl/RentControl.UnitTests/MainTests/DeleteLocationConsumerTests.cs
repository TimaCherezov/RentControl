using Main.Api.Exceptions;
using Main.Api.Features.Locations.Delete;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class DeleteLocationConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<ILocationRepository> _locations = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public DeleteLocationConsumerTests()
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
        var context = TestConsumeContext.For(new DeleteLocationCommand(locationId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenLocationExists_RemovesLocation()
    {
        var location = new LocationBuilder().Build();
        _locations
            .Setup(l => l.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new DeleteLocationCommand(location.Id));

        await consumer.Consume(context.Object);

        _locations.Verify(l => l.Remove(It.Is<Location>(removed => removed.Id == location.Id)), Times.Once);
    }

    private DeleteLocationConsumer CreateConsumer() => new(_factory.Object);
}
