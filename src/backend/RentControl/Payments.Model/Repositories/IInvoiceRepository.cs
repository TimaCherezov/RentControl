using Payments.Model.Models;

namespace Payments.Model.Repositories;

public interface IInvoiceRepository
{
    Task<bool> ExistsByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    void Add(Invoice invoice);
}
