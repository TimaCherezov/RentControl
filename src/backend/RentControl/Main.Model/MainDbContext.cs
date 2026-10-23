using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model;

public class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
