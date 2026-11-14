using EFCore.PostgresExtensions.Extensions;
using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class ProductUnitRepository(MainDbContext dbContext) : IProductUnitRepository
{
    public void Add(ProductUnit productUnit) => dbContext.ProductUnits.Add(productUnit);

    public void Remove(ProductUnit productUnit) => dbContext.ProductUnits.Remove(productUnit);

    /// <summary>
    /// Использовать только внутри UOW/транзакции.
    /// </summary>
    public Task<ProductUnit?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        dbContext
            .ProductUnits.Where(pu => pu.Id == id)
            .ForUpdate()
            .FirstOrDefaultAsync(cancellationToken);

    public Task<List<Guid>> GetAvailableUnitIdsAsync(
        Guid productId,
        DateTimeOffset start,
        DateTimeOffset end,
        Guid? locationId = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.ProductUnits.AsNoTracking();

        if (locationId != null)
            query = query.Where(pu => pu.LocationId == locationId);

        return query
            .Where(pu => pu.ProductId == productId)
            .Where(pu => Enumerable.Contains(StatesExtensions.RentableStates, pu.State))
            .Where(pu =>
                !dbContext.Bookings.Any(b =>
                    b.ProductUnitId == pu.Id
                    && Enumerable.Contains(
                        BookingStatusExtensions.BlockingStatuses,
                        b.BookingStatus
                    )
                    && b.StartAt < end
                    && b.EndAt > start
                )
            )
            .Select(pu => pu.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<TimeSpan> GetMaxAvailableDurationAsync(
        Guid productId,
        DateTimeOffset startAt,
        TimeSpan maxDuration,
        Guid? locationId = null,
        CancellationToken cancellationToken = default
    )
    {
        var availableUnitsIds = await GetAvailableUnitIdsAsync(
            productId,
            startAt,
            startAt,
            locationId,
            cancellationToken
        );

        if (!availableUnitsIds.Any())
            return TimeSpan.Zero;

        var firstBookingByUnit = await GetFirstFutureBookingByUnitAsync(
            availableUnitsIds,
            startAt,
            cancellationToken
        );

        var bestDuration = TimeSpan.Zero;
        foreach (var unitId in availableUnitsIds)
        {
            TimeSpan duration;
            if (firstBookingByUnit.TryGetValue(unitId, out var nextStart))
            {
                duration = nextStart - startAt;
            }
            else
            {
                return maxDuration;
            }

            if (duration > bestDuration)
            {
                bestDuration = duration;
            }
        }

        return bestDuration > maxDuration ? maxDuration : bestDuration;
    }

    public async Task<Guid?> GetBestFitUnitIdAsync(
        Guid productId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        TimeSpan maxDuration,
        Guid? locationId = null,
        CancellationToken cancellationToken = default
    )
    {
        var requiredDuration = endAt - startAt;
        if (requiredDuration > maxDuration)
            return null;

        var availableUnitIds = await GetAvailableUnitIdsAsync(
            productId,
            startAt,
            endAt,
            locationId,
            cancellationToken
        );

        if (!availableUnitIds.Any())
            return null;

        var firstBookingByUnit = await GetFirstFutureBookingByUnitAsync(
            availableUnitIds,
            startAt,
            cancellationToken
        );

        TimeSpan? bestDuration = null;
        Guid? bestUnitId = null;
        foreach (var unitId in availableUnitIds)
        {
            TimeSpan duration;

            if (firstBookingByUnit.TryGetValue(unitId, out var nextStart))
            {
                duration = nextStart - startAt;
            }
            else
            {
                duration = maxDuration;
            }

            if (duration < requiredDuration)
                continue;

            if (bestDuration == null || duration < bestDuration.Value)
            {
                bestDuration = duration;
                bestUnitId = unitId;
            }
        }

        return bestUnitId;
    }

    private Task<Dictionary<Guid, DateTimeOffset>> GetFirstFutureBookingByUnitAsync(
        IReadOnlyCollection<Guid> unitIds,
        DateTimeOffset startAt,
        CancellationToken cancellationToken = default
    )
    {
        if (unitIds.Count == 0)
            return Task.FromResult(new Dictionary<Guid, DateTimeOffset>());

        return dbContext
            .Bookings.AsNoTracking()
            .Where(b =>
                Enumerable.Contains(unitIds, b.ProductUnitId)
                && Enumerable.Contains(BookingStatusExtensions.BlockingStatuses, b.BookingStatus)
                && b.StartAt >= startAt
            )
            .GroupBy(b => b.ProductUnitId)
            .Select(g => new { ProductUnitId = g.Key, FirstStartAt = g.Min(x => x.StartAt) })
            .ToDictionaryAsync(x => x.ProductUnitId, x => x.FirstStartAt, cancellationToken);
    }
}
