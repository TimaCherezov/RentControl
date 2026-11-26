using Main.Api.Features.ProductImages;
using Main.Api.Features.ProductImages.GetAll;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests.Api.ProductImages;

public class GetAllProductImageConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductImageRepository> _productImages = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    public GetAllProductImageConsumerTests()
    {
        _uow.SetupGet(u => u.ProductImages).Returns(_productImages.Object);
        _factory.RunsWith<List<ProductImage>>(_uow);
    }

    [Fact]
    public async Task Consume_WhenImagesExist_RespondsWithAllDtos()
    {
        var productId = Guid.NewGuid();
        var images = new List<ProductImage>
        {
            new ProductImageBuilder().WithProductId(productId).Build(),
            new ProductImageBuilder().WithProductId(productId).Build(),
        };
        _productImages
            .Setup(p => p.GetAllByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(images);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new GetAllProductImageCommand(productId));
        GetAllProductImageResponse? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<GetAllProductImageResponse>()))
            .Callback<GetAllProductImageResponse>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.NotNull(responded);
        Assert.Equal(2, responded.Images.Count);
    }

    private GetAllProductImageConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }));
}
