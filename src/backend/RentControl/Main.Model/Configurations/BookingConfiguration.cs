using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Model.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.StatusChangedAt).HasDefaultValueSql("timezone('utc', now())");

        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.StartAt);

        builder
            .HasOne(b => b.ProductUnit)
            .WithMany()
            .HasForeignKey(b => b.ProductUnitId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(b => b.Location)
            .WithMany()
            .HasForeignKey(b => b.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(b => b.BookingStatus)
            .WithMany()
            .HasForeignKey(b => b.BookingStatusId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.User).WithMany().HasForeignKey(b => b.UserId);
    }
}
