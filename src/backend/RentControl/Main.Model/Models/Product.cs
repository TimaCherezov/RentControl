namespace Main.Model.Models;

/// <summary>
/// Продукт. Отображается на фронте, в карточке.
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    /// <summary>
    /// Актуальная рыночная стоимость продукта.
    /// </summary>
    public decimal MarketValue { get; set; }

    /// <summary>
    /// Сумма депозита. Если не проставлять, то сумма депозита будет считаться динамически, используя рыночную стоимость.
    /// </summary>
    public decimal? DepositOverride { get; set; }
    public decimal DailyRate { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal? WeeklyRate { get; set; }
    public decimal? MonthlyRate { get; set; }
    public string? Description { get; set; }

    public ProductImage? MainImage => Images.FirstOrDefault(i => i.IsMain);
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
