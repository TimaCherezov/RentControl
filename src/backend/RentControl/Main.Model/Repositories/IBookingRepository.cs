using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Booking>> ListAsync(CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByStatusCodeAsync(string code, CancellationToken cancellationToken = default);
    void Update(Booking booking);
}
