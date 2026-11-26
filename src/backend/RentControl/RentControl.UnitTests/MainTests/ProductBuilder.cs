using Main.Model.Models;

namespace RentControl.UnitTests.MainTests;

internal class ProductBuilder
{
    private const decimal DefaultMarketValue = 1000m;

    private Guid _id = Guid.NewGuid();
    private string _name = "Test Product";
    private decimal _marketValue = DefaultMarketValue;
    private decimal? _depositOverride;
    private decimal _dailyRate;
    private decimal _hourlyRate;
    private decimal? _weeklyRate;
    private decimal? _monthlyRate;
    private readonly List<ProductImage> _images = [];

    public ProductBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

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

    public ProductBuilder WithDailyRate(decimal dailyRate)
    {
        _dailyRate = dailyRate;
        return this;
    }

    public ProductBuilder WithHourlyRate(decimal hourlyRate)
    {
        _hourlyRate = hourlyRate;
        return this;
    }

    public ProductBuilder WithWeeklyRate(decimal? weeklyRate)
    {
        _weeklyRate = weeklyRate;
        return this;
    }

    public ProductBuilder WithMonthlyRate(decimal? monthlyRate)
    {
        _monthlyRate = monthlyRate;
        return this;
    }

    public ProductBuilder WithImage(bool isMain = false)
    {
        _images.Add(new ProductImage { IsMain = isMain });
        return this;
    }

    public Product Build() => new()
    {
        Id = _id,
        Name = _name,
        MarketValue = _marketValue,
        DepositOverride = _depositOverride,
        DailyRate = _dailyRate,
        HourlyRate = _hourlyRate,
        WeeklyRate = _weeklyRate,
        MonthlyRate = _monthlyRate,
        Images = _images.ToList(),
    };
}
