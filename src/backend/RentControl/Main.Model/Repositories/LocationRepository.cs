using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class LocationRepository(MainDbContext dbContext) : ILocationRepository
{
    public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Locations.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<PageList<Location>> GetPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var locations = await dbContext
            .Locations
            .AsNoTracking()
            .OrderBy(l => l.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await dbContext.Locations.CountAsync(cancellationToken);

        return new PageList<Location>(locations, totalCount, pageNumber, pageSize);
    }

    public void Add(Location location)
        => dbContext.Locations.Add(location);

    public void Remove(Location location)
        => dbContext.Locations.Remove(location);
}
