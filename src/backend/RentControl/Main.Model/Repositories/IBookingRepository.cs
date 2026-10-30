using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByStatusAsync(
        BookingStatusEnum status,
        CancellationToken cancellationToken = default
    );
    Task<PageList<Booking>> GetPageAsync(
        int pageNumber,
        int pageSize,
        Guid? userId = null,
        BookingStatusEnum? status = null,
        CancellationToken cancellationToken = default
    );
    void Update(Booking booking);
}
