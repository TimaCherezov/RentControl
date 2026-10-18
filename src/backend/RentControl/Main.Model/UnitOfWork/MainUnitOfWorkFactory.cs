using Microsoft.EntityFrameworkCore;
using RentControl.Common.UnitOfWork;

namespace Main.Model.UnitOfWork;

internal sealed class MainUnitOfWorkFactory(IDbContextFactory<MainDbContext> dbContextFactory)
    : UnitOfWorkFactory<MainDbContext, IMainUnitOfWork>(dbContextFactory)
{
    protected override IMainUnitOfWork CreateUnitOfWork(MainDbContext context) => new MainUnitOfWork(context);
}
