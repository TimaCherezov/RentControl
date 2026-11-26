using Main.Model.UnitOfWork;
using Moq;
using RentControl.Common.UnitOfWork;

namespace RentControl.UnitTests.MainTests;

internal static class UnitOfWorkFactoryMockExtensions
{
    public static Mock<IUnitOfWorkFactory<IMainUnitOfWork>> RunsWith(this Mock<IUnitOfWorkFactory<IMainUnitOfWork>> factory, Mock<IMainUnitOfWork> uow)
    {
        factory
            .Setup(f => f.ExecuteAsync(It.IsAny<Func<IMainUnitOfWork, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<IMainUnitOfWork, Task>, CancellationToken>((operation, ct) => operation(uow.Object));
        return factory;
    }

    public static Mock<IUnitOfWorkFactory<IMainUnitOfWork>> RunsWith<T>(this Mock<IUnitOfWorkFactory<IMainUnitOfWork>> factory, Mock<IMainUnitOfWork> uow)
    {
        factory
            .Setup(f => f.ExecuteAsync(It.IsAny<Func<IMainUnitOfWork, Task<T>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<IMainUnitOfWork, Task<T>>, CancellationToken>((operation, ct) => operation(uow.Object));
        return factory;
    }
}
