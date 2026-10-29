using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class BookingStatusRepository(MainDbContext dbContext) : IBookingStatusRepository
{
    public Task<BookingStatus?> GetByIdAsync(
        BookingStatusEnum id,
        CancellationToken cancellationToken = default
    ) => dbContext.BookingStatuses.FirstOrDefaultAsync(bs => bs.Id == id, cancellationToken);
}
