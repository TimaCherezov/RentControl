using Payments.Model.Repositories;

namespace Payments.Model.UnitOfWork;

internal sealed class PaymentsUnitOfWork(PaymentsDbContext dbContext) : IPaymentsUnitOfWork
{
    public PaymentsDbContext Context { get; } = dbContext;

    public IInvoiceRepository Invoices { get; } = new InvoiceRepository(dbContext);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);
}
