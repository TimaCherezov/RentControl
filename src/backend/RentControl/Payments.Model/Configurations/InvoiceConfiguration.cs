using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Model.Models;

namespace Payments.Model.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasIndex(invoice => invoice.BookingId).IsUnique();

        builder.Property(invoice => invoice.Amount).HasPrecision(18, 2);

        builder.Property(invoice => invoice.Status)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();
    }
}
