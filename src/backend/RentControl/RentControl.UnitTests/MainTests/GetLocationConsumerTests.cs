using Main.Api.Exceptions;
using Main.Api.Features.Locations;
using Main.Api.Features.Locations.Get;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class GetLocationConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<ILocationRepository> _locations = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetLocationConsumerTests()
    {
        _uow.SetupGet(u => u.Locations).Returns(_locations.Object);
        _factory.RunsWith<Location>(_uow);
    }

    [Fact]
    public async Task Consume_WhenLocationNotFound_ThrowsObjectNotFoundException()
    {
        var locationId = Guid.NewGuid();
        _locations
            .Setup(l => l.GetByIdAsync(locationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetLocationCommand(locationId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenLocationExists_RespondsWithLocationDto()
    {
        var location = new LocationBuilder().WithName("Central Point").WithAddress("Main Street 1").Build();
        _locations
            .Setup(l => l.GetByIdAsync(location.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetLocationCommand(location.Id));
        LocationDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<LocationDto>()))
            .Callback<LocationDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(location.Id, responded.Id);
        Assert.Equal("Central Point", responded.Name);
        Assert.Equal("Main Street 1", responded.Address);
    }

    private GetLocationConsumer CreateConsumer() => new(_factory.Object);
}
