using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Booking>> ListAsync(CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByStatusIdAsync(BookingStatusEnum statusId, CancellationToken cancellationToken = default);
    Task<PageList<Booking>> GetPageAsync(int pageNumber, int pageSize, Guid? userId = null, BookingStatusEnum? statusId = null, CancellationToken cancellationToken = default);
    void Update(Booking booking);
}
