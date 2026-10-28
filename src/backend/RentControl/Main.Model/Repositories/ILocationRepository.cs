using Main.Model.Models;

namespace Main.Model.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Location location);
    void Update(Location location);
}
