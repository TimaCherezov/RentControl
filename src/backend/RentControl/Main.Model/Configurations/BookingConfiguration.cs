using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Model.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasOne(b => b.ProductUnit).WithMany().HasForeignKey(b => b.ProductUnitId);
        builder.HasOne(b => b.Location).WithMany().HasForeignKey(b => b.LocationId);
        builder.HasOne(b => b.BookingStatus).WithMany().HasForeignKey(b => b.BookingStatusId);
        builder.HasOne(b => b.User).WithMany().HasForeignKey(b => b.UserId);
    }
}
