using Main.Api.Exceptions;
using Main.Api.Features.ProductImages;
using Main.Api.Features.ProductImages.Create;
using Main.Model.Models;
using Main.Model.Repositories;
using Main.Model.UnitOfWork;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

public class CreateProductImageConsumerTests
{
    private readonly Mock<IMainUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IProductImageRepository> _productImages = new();
    private readonly Mock<IUnitOfWorkFactory<IMainUnitOfWork>> _factory = new();

    private ProductImage? _addedImage;

    public CreateProductImageConsumerTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _uow.SetupGet(u => u.ProductImages).Returns(_productImages.Object);
        _factory.RunsWith<ProductImage>(_uow);
        _productImages
            .Setup(p => p.Add(It.IsAny<ProductImage>()))
            .Callback<ProductImage>(image => _addedImage = image);
    }

    [Fact]
    public async Task Consume_WhenProductNotFound_ThrowsObjectNotFoundException()
    {
        var productId = Guid.NewGuid();
        _products
            .Setup(p => p.GetAndLockForUpdateByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new CreateProductImageCommand(productId, "images/photo.jpg", null));

        await Assert.ThrowsAsync<ObjectNotFoundException>(() => consumer.Consume(context.Object));
    }

    [Fact]
    public async Task Consume_WhenProductHasNoMainImage_NewImageBecomesMain()
    {
        var productId = Guid.NewGuid();
        ArrangeProduct(productId, hasMainImage: false);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new CreateProductImageCommand(productId, "images/photo.jpg", null));
        ProductImageDto? responded = null;
        context
            .Setup(c => c.RespondAsync(It.IsAny<ProductImageDto>()))
            .Callback<ProductImageDto>(response => responded = response)
            .Returns(Task.CompletedTask);

        await consumer.Consume(context.Object);

        Assert.True(_addedImage?.IsMain);
        Assert.True(responded?.IsMain);
        Assert.Equal("http://images.test/images/photo.jpg", responded?.Url);
    }

    [Fact]
    public async Task Consume_WhenProductHasMainImage_NewImageIsNotMain()
    {
        var productId = Guid.NewGuid();
        ArrangeProduct(productId, hasMainImage: true);
        var consumer = CreateConsumer();
        var context = TestConsumeContext.For(new CreateProductImageCommand(productId, "images/photo.jpg", null));

        await consumer.Consume(context.Object);

        Assert.False(_addedImage?.IsMain);
    }

    private void ArrangeProduct(Guid productId, bool hasMainImage)
    {
        _products
            .Setup(p => p.GetAndLockForUpdateByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductBuilder().WithId(productId).Build());
        _productImages
            .Setup(p => p.GetMainByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hasMainImage ? new ProductImageBuilder().AsMain().WithProductId(productId).Build() : null);
    }

    private CreateProductImageConsumer CreateConsumer() => new(
        _factory.Object,
        Options.Create(new ImageStorageOptions { ImageBaseUrl = "http://images.test" }));
}
