using Main.Api.Features.Locations;
using Main.Api.Features.Locations.GetPage;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Moq;
using RentControl.Common.DTOs;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Locations;

public class GetPageLocationConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<ILocationRepository> _locations = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetPageLocationConsumerTests()
    {
        _uow.SetupGet(u => u.Locations).Returns(_locations.Object);
        _factory.RunsWith<PageList<Location>>(_uow);
    }

    [Fact]
    public async Task Consume_WhenPageExists_RespondsWithPagedLocationDtos()
    {
        var locations = new List<Location>
        {
            new LocationBuilder().Build(),
            new LocationBuilder().Build(),
        };
        _locations
            .Setup(l => l.GetPageAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageList<Location>(locations, totalCount: 3, pageNumber: 1, pageSize: 2));
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetPageLocationCommand(1, 2));
        PagedResponse<LocationDto>? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<PagedResponse<LocationDto>>()))
            .Callback<PagedResponse<LocationDto>>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(2, responded.Items.Count);
        Assert.Equal(3, responded.TotalCount);
        Assert.Equal(2, responded.TotalPages);
    }

    private GetPageLocationConsumer CreateConsumer() => new(_factory.Object);
}
