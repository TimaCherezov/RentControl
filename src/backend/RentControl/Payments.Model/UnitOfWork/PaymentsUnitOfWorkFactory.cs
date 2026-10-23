using Microsoft.EntityFrameworkCore;
using RentControl.Common.UnitOfWork;

namespace Payments.Model.UnitOfWork;

internal sealed class PaymentsUnitOfWorkFactory(IDbContextFactory<PaymentsDbContext> dbContextFactory)
    : UnitOfWorkFactory<PaymentsDbContext, IPaymentsUnitOfWork>(dbContextFactory)
{
    protected override IPaymentsUnitOfWork CreateUnitOfWork(PaymentsDbContext context) => new PaymentsUnitOfWork(context);
}
