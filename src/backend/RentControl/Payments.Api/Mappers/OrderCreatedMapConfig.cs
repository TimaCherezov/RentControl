using Mapster;
using Payments.Model.Models;
using RentControl.Common.IntegrationEvents;

namespace Payments.Api.Mappers;

/// <summary>
/// OrderCreated → Invoice: суммы и бронь берутся из события,
/// служебные поля счёта проставляются при выставлении.
/// </summary>
public static class OrderCreatedMapConfig
{
    public static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<OrderCreated, Invoice>()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.BookingId, src => src.BookingId)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Status, _ => InvoiceStatus.Pending)
            .Map(dest => dest.CreatedAt, _ => DateTimeOffset.UtcNow);
    }
}
