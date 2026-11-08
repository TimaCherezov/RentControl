using Payments.Model.Repositories;

namespace Payments.Model.UnitOfWork;

public interface IPaymentsUnitOfWork
{
    PaymentsDbContext Context { get; }

    IInvoiceRepository Invoices { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
