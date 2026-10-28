namespace Main.Model.Models;

/// <summary>
/// Вронирование для продукта.
/// </summary>
public class Booking
{
    public Guid Id { get; set; }
    public ProductUnit ProductUnit { get; set; } = null!;
    public Guid ProductUnitId { get; set; }

    /// <summary>
    /// Время начала бронирования.
    /// </summary>
    public DateTimeOffset StartAt { get; set; }

    /// <summary>
    /// Время окончания бронироваия (планируемое).
    /// </summary>
    public DateTimeOffset EndAt { get; set; }
    public Location Location { get; set; } = null!;
    public Guid LocationId { get; set; }

    /// <summary>
    /// Итоговая дата и время завершения бронирования. Может отличаться от EndAt, если пользователь задержит товар.
    /// </summary>
    public DateTimeOffset? ActualReturnAt { get; set; }

    /// <summary>
    /// Общая сумма бронирования с учётом возможной задержки. Проставляется после заверщения бронирования.
    /// </summary>
    public decimal? TotalRentalCost { get; set; }

    /// <summary>
    /// Сумма внесенного депозита на во время создания бронирования. Необходима для возврата средств.
    /// </summary>
    public decimal DepositAmount { get; set; }
    public BookingStatus BookingStatus { get; set; } = null!;
    public DateTimeOffset StatusChangedAt { get; set; }
    public BookingStatusEnum BookingStatusId { get; set; }
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
}
