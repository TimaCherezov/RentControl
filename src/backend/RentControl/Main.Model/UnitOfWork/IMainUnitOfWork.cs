namespace Main.Model.UnitOfWork;

public interface IMainUnitOfWork
{
    MainDbContext Context { get; }
}
