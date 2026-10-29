using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class BookingRepository(MainDbContext dbContext) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Bookings
            .Include(b => b.BookingStatus)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public Task<List<Booking>> ListAsync(CancellationToken cancellationToken = default)
        => dbContext.Bookings
            .Include(b => b.BookingStatus)
            .ToListAsync(cancellationToken);

    public async Task<List<Booking>> GetByStatusCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var status = await dbContext.BookingStatuses
            .FirstOrDefaultAsync(bs => bs.Code == code, cancellationToken);

        if (status is null)
            return [];

        return await dbContext.Bookings
            .Include(b => b.BookingStatus)
            .Where(b => b.BookingStatusId == status.Id)
            .ToListAsync(cancellationToken);
    }

    public void Update(Booking booking)
        => dbContext.Bookings.Update(booking);
}
