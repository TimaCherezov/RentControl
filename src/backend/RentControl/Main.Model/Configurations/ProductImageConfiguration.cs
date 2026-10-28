using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Model.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.Property(pi => pi.StorageKey).HasMaxLength(255);
        builder.Property(pi => pi.AltText).HasMaxLength(255);

        builder.HasOne(pi => pi.Product).WithMany().HasForeignKey(pi => pi.ProductId);
    }
}
