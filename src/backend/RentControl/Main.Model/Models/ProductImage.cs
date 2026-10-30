namespace Main.Model.Models;

/// <summary>
/// Изображение продукта.
/// </summary>
public class ProductImage
{
    public Guid Id { get; set; }
    public Product Product { get; set; } = null!;
    public Guid ProductId { get; set; }

    /// <summary>
    /// Относительный путь до изображения. BaseUrl или BasePath может меняться (для мобильности в использовании источников хранения).
    /// </summary>
    public string StorageKey { get; set; } = null!;

    /// <summary>
    /// Флаг. Указывает на главное изображение для продукта, отображаемое на странице с продуктами. 
    /// В карточке продукта идет самое первое. Остальные - по времени загрузки.
    /// </summary>
    public bool IsMain { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Текст для отображения в случае неудачной загрузки изоюражения.
    /// </summary>
    public string? AltText { get; set; }
}
