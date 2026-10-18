namespace RentControl.Common.UnitOfWork;

public interface IUnitOfWorkFactory<TUnitOfWork>
{
    Task ExecuteAsync(Func<TUnitOfWork, Task> operation, CancellationToken ct = default);

    Task<T> ExecuteAsync<T>(Func<TUnitOfWork, Task<T>> operation, CancellationToken ct = default);
}
