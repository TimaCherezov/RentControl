using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class BookingStatusRepository(MainDbContext dbcontext) : IBookingStatusRepository
{
    public Task<BookingStatus?> GetByIdAsync(
        BookingStatusEnum id,
        CancellationToken cancellationToken = default
    ) => dbcontext.BookingStatuses.FirstOrDefaultAsync(bs => bs.Id == id, cancellationToken);
}
