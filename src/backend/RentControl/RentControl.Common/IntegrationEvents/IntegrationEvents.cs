namespace RentControl.Common.IntegrationEvents;

/// <summary>
/// Main создал заказ (бронь) — Payments должен выставить счёт.
/// </summary>
public record OrderCreated(Guid BookingId, decimal Amount);

/// <summary>
/// Payments выставил счёт по заказу (1:1 с бронью, идентификатор счёта —
/// внутреннее понятие Payments). Main отмечает бронь как готовую к оплате.
/// </summary>
public record InvoiceCreated(Guid BookingId);

/// <summary>
/// Счёт оплачен. Main переводит бронь в подтверждённые.
/// </summary>
public record PaymentSucceeded(Guid BookingId);
/// <summary>
/// Заказ изменился (статус, готовность счёта) — событие для Realtime (SignalR → фронтенд).
/// </summary>
public record OrderChanged(Guid BookingId, Guid UserId, string Status);
