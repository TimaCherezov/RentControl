using Main.Model.Models;

namespace Main.Model.Repositories.Interfaces;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Location location, CancellationToken cancellationToken = default);
}
