using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Model.Configurations;

public class BookingStatusConfiguration : IEntityTypeConfiguration<BookingStatus>
{
    public void Configure(EntityTypeBuilder<BookingStatus> builder)
    {
        builder.HasIndex(bs => bs.Code).IsUnique();
        builder.Property(bs => bs.Code).HasMaxLength(50);
        builder.Property(bs => bs.Name).HasMaxLength(50);
        builder.Property(bs => bs.Id).HasConversion<int>();

        builder.HasData([
                new BookingStatus { Id = BookingStatusEnum.ACTIVE, Code = "ACTIVE", Name = "Active" },
                new BookingStatus { Id = BookingStatusEnum.CANCELLED, Code = "CANCELLED", Name = "Cancelled" },
                new BookingStatus { Id = BookingStatusEnum.COMPLETED, Code = "COMPLETED", Name = "Completed" },
                new BookingStatus { Id = BookingStatusEnum.CONFIRMED, Code = "CONFIRMED", Name = "Confirmed" },
                new BookingStatus { Id = BookingStatusEnum.OVERDUE, Code = "OVERDUE", Name = "Overdue" },
                new BookingStatus { Id = BookingStatusEnum.PENDING, Code = "PENDING", Name = "Pending" },
                new BookingStatus { Id = BookingStatusEnum.READY_FOR_PICKUP, Code = "READY_FOR_PICKUP", Name = "Ready for pickup" },
                new BookingStatus { Id = BookingStatusEnum.RETURNED, Code = "RETURNED", Name = "Returned" }
        ]);
    }
}
