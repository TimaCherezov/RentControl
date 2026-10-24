namespace Payments.Model.UnitOfWork;

internal sealed class PaymentsUnitOfWork(PaymentsDbContext dbContext) : IPaymentsUnitOfWork
{
    public PaymentsDbContext Context { get; } = dbContext;
}
