using MassTransit;
using Moq;

namespace RentControl.UnitTests.MainTests;

internal static class TestConsumeContext
{
    public static Mock<ConsumeContext<T>> For<T>(T message) where T : class
    {
        var context = new Mock<ConsumeContext<T>>();
        context.SetupGet(c => c.Message).Returns(message);
        context.SetupGet(c => c.CancellationToken).Returns(CancellationToken.None);
        return context;
    }
}
