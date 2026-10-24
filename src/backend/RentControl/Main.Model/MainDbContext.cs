using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model;

public class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingStatus> BookingStatuses => Set<BookingStatus>();
    public DbSet<State> States => Set<State>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductUnit> ProductUnits => Set<ProductUnit>();
    public DbSet<Location> Locations => Set<Location>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
