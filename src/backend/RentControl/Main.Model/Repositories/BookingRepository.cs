using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class BookingRepository(MainDbContext dbContext) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Bookings
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<List<Booking>> GetByStatusAsync(BookingStatusEnum status, CancellationToken cancellationToken = default)
    {
        return await dbContext.Bookings
            .Where(b => b.BookingStatus == status)
            .ToListAsync(cancellationToken);
    }

    public void Update(Booking booking)
        => dbContext.Bookings.Update(booking);

    public async Task<PageList<Booking>> GetPageAsync(
        int pageNumber,
        int pageSize,
        Guid? userId = null,
        BookingStatusEnum? status = null,
        CancellationToken cancellationToken = default
    )
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var query = dbContext
            .Bookings.Include(b => b.ProductUnit.Product.Images.Where(i => i.IsMain))
            .AsQueryable()
            .AsNoTracking();

        if (userId is not null)
            query = query.Where(b => b.UserId == userId);
        if (status is not null)
            query = query.Where(b => b.BookingStatus == status);

        var bookings = await query
            .OrderBy(b => b.StartAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await query.CountAsync(cancellationToken);

        return new PageList<Booking>(bookings, totalCount, pageNumber, pageSize);
    }

    public void Add(Booking booking)
        => dbContext.Bookings.Add(booking);
}
