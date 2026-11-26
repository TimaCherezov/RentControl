using Main.Api.Features.Bookings;
using Main.Api.Features.Bookings.GetPage;
using Main.Api.Features.ProductImages;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.DTOs;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.Bookings;

public class GetPageBookingConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetPageBookingConsumerTests()
    {
        _uow.SetupGet(u => u.Bookings).Returns(_bookings.Object);
        _factory.RunsWith<PageList<Booking>>(_uow);
    }

    [Fact]
    public async Task Consume_WhenPageExists_RespondsWithPagedBookingDtos()
    {
        var userId = Guid.NewGuid();
        var bookings = new List<Booking>
        {
            new BookingBuilder().WithUserId(userId).Build(),
            new BookingBuilder().WithUserId(userId).Build(),
        };
        _bookings
            .Setup(b => b.GetPageAsync(2, 2, userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PageList<Booking>(bookings, totalCount: 5, pageNumber: 2, pageSize: 2));
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetPageBookingCommand(2, 2, null, userId));
        PagedResponse<BookingDto>? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<PagedResponse<BookingDto>>()))
            .Callback<PagedResponse<BookingDto>>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(2, responded.Items.Count);
        Assert.Equal(5, responded.TotalCount);
        Assert.Equal(2, responded.PageNumber);
        Assert.Equal(2, responded.PageSize);
        Assert.Equal(3, responded.TotalPages);
    }

    private GetPageBookingConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }));
}
