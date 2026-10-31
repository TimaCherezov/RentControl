using Main.Model.Models;
using Main.Model.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class LocationRepository(MainDbContext dbContext) : ILocationRepository
{
    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Locations.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
        => await dbContext.Locations.AddAsync(location, cancellationToken);

    public void Update(Location location)
        => dbContext.Locations.Update(location);
}
