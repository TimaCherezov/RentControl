using Main.Api.Features.Locations;
using Main.Api.Features.Locations.Create;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class CreateLocationConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<ILocationRepository> _locations = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    private Location? _addedLocation;

    public CreateLocationConsumerTests()
    {
        _uow.SetupGet(u => u.Locations).Returns(_locations.Object);
        _factory.RunsWith<Location>(_uow);
        _locations
            .Setup(l => l.Add(It.IsAny<Location>()))
            .Callback<Location>(location => _addedLocation = location);
    }

    [Fact]
    public async Task Consume_WhenValid_AddsLocation()
    {
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new CreateLocationCommand("Central Point", "Main Street 1"));

        await consumer.Consume(context.Object);

        Assert.NotNull(_addedLocation);
        Assert.Equal("Central Point", _addedLocation.Name);
        Assert.Equal("Main Street 1", _addedLocation.Address);
    }

    [Fact]
    public async Task Consume_WhenValid_RespondsWithLocationDto()
    {
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new CreateLocationCommand("Central Point", "Main Street 1"));
        LocationDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<LocationDto>()))
            .Callback<LocationDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal("Central Point", responded.Name);
        Assert.Equal("Main Street 1", responded.Address);
    }

    private CreateLocationConsumer CreateConsumer() => new(_factory.Object);
}
