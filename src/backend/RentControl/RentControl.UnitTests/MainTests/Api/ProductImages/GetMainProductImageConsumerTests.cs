using RentControl.ServiceDefaults.Exceptions;
using Main.Api.Features.ProductImages;
using Main.Api.Features.ProductImages.GetMain;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.ProductImages;

public class GetMainProductImageConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductImageRepository> _productImages = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetMainProductImageConsumerTests()
    {
        _uow.SetupGet(u => u.ProductImages).Returns(_productImages.Object);
        _factory.RunsWith<ProductImage>(_uow);
    }

    [Fact]
    public async Task Consume_WhenProductHasNoMainImage_ThrowsObjectNotFoundException()
    {
        var productId = Guid.NewGuid();
        _productImages
            .Setup(p => p.GetMainByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductImage?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetMainProductImageCommand(productId));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenMainImageExists_RespondsWithDtoWithBuiltUrl()
    {
        var productId = Guid.NewGuid();
        var image = new ProductImageBuilder().AsMain().WithProductId(productId).Build();
        _productImages
            .Setup(p => p.GetMainByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetMainProductImageCommand(productId));
        ProductImageDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<ProductImageDto>()))
            .Callback<ProductImageDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(image.Id, responded.Id);
        Assert.Equal("http://images.test/images/photo.jpg", responded.Url);
    }

    private GetMainProductImageConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }));
}
