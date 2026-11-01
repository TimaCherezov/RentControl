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
        builder.Property(pi => pi.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(pi => new { pi.ProductId, pi.IsMain }).IsUnique().HasFilter("\"IsMain\"=true");

        builder.HasOne(pi => pi.Product).WithMany(p => p.Images).HasForeignKey(pi => pi.ProductId);
    }
}
