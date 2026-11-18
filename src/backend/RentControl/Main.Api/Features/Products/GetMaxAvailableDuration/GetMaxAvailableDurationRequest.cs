namespace Main.Api.Features.Products.GetMaxAvailableDuration;

public sealed class GetMaxAvailableDurationRequest
{
    public DateTimeOffset StartAt { get; set; }
    public Guid? LocationId { get; set; }
}
