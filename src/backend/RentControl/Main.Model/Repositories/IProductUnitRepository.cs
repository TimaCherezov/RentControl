using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IProductUnitRepository
{
    Task<ProductUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(ProductUnit productUnit);
    void Remove(ProductUnit productUnit);
    Task<TimeSpan> GetMaxAvailableDurationAsync(
        Guid productId,
        DateTimeOffset startAt,
        TimeSpan maxDuration,
        Guid? locationId = null,
        CancellationToken cancellationToken = default
    );
    Task<Guid?> GetBestFitUnitIdAsync(
        Guid productId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        TimeSpan maxDuration,
        Guid? locationId = null,
        CancellationToken cancellationToken = default
    );
}
