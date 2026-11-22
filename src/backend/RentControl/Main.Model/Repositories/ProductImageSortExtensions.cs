using Main.Model.Models;

namespace Main.Model.Repositories;

public static class ProductImageSortExtensions
{
    /// <summary>
    /// Порядок картинок для отображения: главная первая, затем по времени загрузки.
    /// Id - для детерминированности при равных CreatedAt.
    /// </summary>
    public static IOrderedEnumerable<ProductImage> SortForDisplay(
        this IEnumerable<ProductImage> images
    ) =>
        images
            .OrderByDescending(i => i.IsMain)
            .ThenBy(i => i.CreatedAt)
            .ThenBy(i => i.Id);
}
