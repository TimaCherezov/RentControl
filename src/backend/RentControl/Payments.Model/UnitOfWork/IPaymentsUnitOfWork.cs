namespace Payments.Model.UnitOfWork;

public interface IPaymentsUnitOfWork
{
    PaymentsDbContext Context { get; }
}
