using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

internal class ProductBuilder
{
    private const decimal DefaultMarketValue = 1000m;

    private decimal _marketValue = DefaultMarketValue;
    private decimal? _depositOverride;
    private readonly List<ProductImage> _images = [];

    public ProductBuilder WithMarketValue(decimal marketValue)
    {
        _marketValue = marketValue;
        return this;
    }

    public ProductBuilder WithDepositOverride(decimal? depositOverride)
    {
        _depositOverride = depositOverride;
        return this;
    }

    public ProductBuilder WithImage(bool isMain = false)
    {
        _images.Add(new ProductImage { IsMain = isMain });
        return this;
    }

    public Product Build() => new()
    {
        MarketValue = _marketValue,
        DepositOverride = _depositOverride,
        Images = _images,
    };
}
