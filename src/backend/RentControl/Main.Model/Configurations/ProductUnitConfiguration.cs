using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Model.Configurations;

public class ProductUnitConfiguration : IEntityTypeConfiguration<ProductUnit>
{
    public void Configure(EntityTypeBuilder<ProductUnit> builder)
    {
        builder.Property(pu => pu.SerialNumber).HasMaxLength(255);
        builder.HasIndex(pu => pu.SerialNumber).IsUnique();

        builder.HasOne(pu => pu.Product).WithMany().HasForeignKey(pu => pu.ProductId);
        builder.HasOne(pu => pu.State).WithMany().HasForeignKey(pu => pu.StateId);
        builder.HasOne(pu => pu.Location).WithMany().HasForeignKey(pu => pu.LocationId);
    }
}
