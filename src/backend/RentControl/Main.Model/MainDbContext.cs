using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model;

public class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Booking> Booking => Set<Booking>();
    public DbSet<BookingStatus> BookingStatus => Set<BookingStatus>();
    public DbSet<Role> Role => Set<Role>();
    public DbSet<State> State => Set<State>();
    public DbSet<Product> Product => Set<Product>();
    public DbSet<ProductImage> ProductImage => Set<ProductImage>();
    public DbSet<ProductUnit> ProductUnit => Set<ProductUnit>();
    public DbSet<Location> Location => Set<Location>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
