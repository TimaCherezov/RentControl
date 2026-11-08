namespace Payments.Model.Models;

public enum InvoiceStatus
{
    Pending = 1,
    Paid = 2
}

public class Invoice
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public decimal Amount { get; set; }

    public InvoiceStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
