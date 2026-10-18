namespace Main.Model.UnitOfWork;

internal sealed class MainUnitOfWork(MainDbContext dbContext) : IMainUnitOfWork
{
    public MainDbContext Context { get; } = dbContext;
}
