using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IBookingStatusRepository
{
    Task<BookingStatus?> GetByIdAsync(BookingStatusEnum id, CancellationToken cancellationToken = default);
}
