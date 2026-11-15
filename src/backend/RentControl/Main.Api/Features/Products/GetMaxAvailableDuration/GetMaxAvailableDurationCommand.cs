namespace Main.Api.Features.Products.GetMaxAvailableDuration;

public record GetMaxAvailableDurationCommand(Guid Id, DateTimeOffset StartAt, Guid? LocationId);
