using Microsoft.EntityFrameworkCore;

namespace RentControl.Common.UnitOfWork;

public abstract class UnitOfWorkFactory<TContext, TUnitOfWork>(
    IDbContextFactory<TContext> dbContextFactory)
    : IUnitOfWorkFactory<TUnitOfWork>
    where TContext : DbContext
{
    protected abstract TUnitOfWork CreateUnitOfWork(TContext context);

    public async Task ExecuteAsync(Func<TUnitOfWork, Task> operation, CancellationToken ct = default)
    {
        await using var strategyContext = await dbContextFactory.CreateDbContextAsync(ct);
        var strategy = strategyContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                await operation(CreateUnitOfWork(dbContext));

                await dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    public async Task<T> ExecuteAsync<T>(Func<TUnitOfWork, Task<T>> operation, CancellationToken ct = default)
    {
        await using var strategyContext = await dbContextFactory.CreateDbContextAsync(ct);
        var strategy = strategyContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var result = await operation(CreateUnitOfWork(dbContext));

                await dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }
}
