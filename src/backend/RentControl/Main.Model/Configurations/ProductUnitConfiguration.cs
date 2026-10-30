using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Model.Configurations;

public class ProductUnitConfiguration : IEntityTypeConfiguration<ProductUnit>
{
    public void Configure(EntityTypeBuilder<ProductUnit> builder)
    {
        builder.Property(pu => pu.SerialNumber).HasMaxLength(255);
        builder.Property(pu => pu.State).HasConversion<string>();

        builder.HasIndex(pu => new { pu.ProductId, pu.SerialNumber }).IsUnique();

        builder
            .HasOne(pu => pu.Product)
            .WithMany()
            .HasForeignKey(pu => pu.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(pu => pu.Location)
            .WithMany()
            .HasForeignKey(pu => pu.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
