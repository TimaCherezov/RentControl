using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class LocationRepository(MainDbContext dbContext) : ILocationRepository
{
    public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Locations.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public void Add(Location location)
        => dbContext.Locations.Add(location);

    public void Remove(Location location)
        => dbContext.Locations.Remove(location);
}
