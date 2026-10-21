using Main.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Main.Model.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        
        builder.Property(x => x.TokenHash).HasMaxLength(100);
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.Ignore(x => x.IsActive);
    }
}
