using Main.Model.Models;

namespace Main.Model.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetListAsync(CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByStatusCodeAsync(string code, CancellationToken cancellationToken = default);
    void Update(Booking booking);
}
