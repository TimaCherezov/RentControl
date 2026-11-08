using Microsoft.EntityFrameworkCore;
using Payments.Model.Models;

namespace Payments.Model.Repositories;

public class InvoiceRepository(PaymentsDbContext dbContext) : IInvoiceRepository
{
    public Task<bool> ExistsByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        => dbContext.Invoices.AnyAsync(invoice => invoice.BookingId == bookingId, cancellationToken);

    public Task<Invoice?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        => dbContext.Invoices
            .FirstOrDefaultAsync(invoice => invoice.BookingId == bookingId, cancellationToken);

    public void Add(Invoice invoice)
        => dbContext.Invoices.Add(invoice);
}
