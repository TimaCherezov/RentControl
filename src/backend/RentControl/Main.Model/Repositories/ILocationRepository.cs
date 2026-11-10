using Main.Model.Models;

namespace Main.Model.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageList<Location>> GetPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );
    void Add(Location location);
    void Remove(Location location);
}
