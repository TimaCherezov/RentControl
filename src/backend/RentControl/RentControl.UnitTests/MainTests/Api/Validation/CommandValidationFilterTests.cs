using FluentValidation;
using FluentValidation.Results;
using Main.Api.Features.Bookings.Create;
using Main.Api.Features.Validation;
using MassTransit;
using Moq;

namespace RentControl.UnitTests.MainTests.Api.Validation;

public class CommandValidationFilterTests
{
    private static readonly CreateBookingCommand Command = new(
        Guid.NewGuid(),
        null,
        DateTimeOffset.UtcNow.AddDays(1),
        DateTimeOffset.UtcNow.AddDays(2),
        Guid.NewGuid());

    [Fact]
    public async Task Send_WhenValidationFails_AggregatesAllErrorsAndDoesNotCallNext()
    {
        var filter = new CommandValidationFilter<CreateBookingCommand>(
        [
            ValidatorReturning(new ValidationFailure("ProductId", "ProductId is invalid")),
            ValidatorReturning(new ValidationFailure("StartAt", "StartAt is in the past")),
        ]);
        var sendContext = CreateSendContext();
        var next = CreateNext();

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => filter.Send(sendContext.Object, next.Object));

        Assert.Equal(2, exception.Errors.Count());
        next.Verify(p => p.Send(It.IsAny<SendContext<CreateBookingCommand>>()), Times.Never);
    }

    [Fact]
    public async Task Send_WhenMessageIsValid_CallsNext()
    {
        var filter = new CommandValidationFilter<CreateBookingCommand>(
        [
            ValidatorReturning(),
            ValidatorReturning(),
        ]);
        var sendContext = CreateSendContext();
        var next = CreateNext();

        await filter.Send(sendContext.Object, next.Object);

        next.Verify(p => p.Send(sendContext.Object), Times.Once);
    }

    private static IValidator<CreateBookingCommand> ValidatorReturning(params ValidationFailure[] failures)
    {
        var validator = new Mock<IValidator<CreateBookingCommand>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateBookingCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));
        return validator.Object;
    }

    private static Mock<SendContext<CreateBookingCommand>> CreateSendContext()
    {
        var sendContext = new Mock<SendContext<CreateBookingCommand>>();
        sendContext.SetupGet(c => c.Message).Returns(Command);
        sendContext.SetupGet(c => c.CancellationToken).Returns(CancellationToken.None);
        return sendContext;
    }

    private static Mock<IPipe<SendContext<CreateBookingCommand>>> CreateNext()
    {
        var next = new Mock<IPipe<SendContext<CreateBookingCommand>>>();
        next.Setup(p => p.Send(It.IsAny<SendContext<CreateBookingCommand>>())).Returns(Task.CompletedTask);
        return next;
    }
}
