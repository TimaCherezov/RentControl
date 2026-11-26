using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

public class ProductTests
{
    [Fact]
    public void CalculateDeposit_WhenDepositOverrideIsSet_ReturnsOverride()
    {
        var product = new ProductBuilder()
            .WithMarketValue(1000m)
            .WithDepositOverride(150m)
            .Build();

        var deposit = product.CalculateDeposit(0.2m);

        Assert.Equal(150m, deposit);
    }

    [Fact]
    public void CalculateDeposit_WhenDepositOverrideIsZero_ReturnsZero()
    {
        var product = new ProductBuilder()
            .WithDepositOverride(0m)
            .Build();

        var deposit = product.CalculateDeposit(0.2m);

        Assert.Equal(0m, deposit);
    }

    [Fact]
    public void CalculateDeposit_WhenDepositOverrideIsNotSet_ReturnsMarketValueTimesRate()
    {
        var product = new ProductBuilder()
            .WithMarketValue(1000m)
            .Build();

        var deposit = product.CalculateDeposit(0.2m);

        Assert.Equal(200m, deposit);
    }

    [Fact]
    public void MainImage_WhenMainImageExists_ReturnsThatImage()
    {
        var product = new ProductBuilder()
            .WithImage(isMain: true)
            .WithImage()
            .Build();

        var mainImage = product.MainImage;

        Assert.NotNull(mainImage);
        Assert.True(mainImage.IsMain);
    }

    [Fact]
    public void MainImage_WhenNoMainImage_ReturnsNull()
    {
        var product = new ProductBuilder()
            .WithImage()
            .WithImage()
            .Build();

        Assert.Null(product.MainImage);
    }
}
