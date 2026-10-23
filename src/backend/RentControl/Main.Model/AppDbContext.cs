using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<RefreshToken> RefreshTokens { get; set; }
    public required DbSet<Booking> Booking { get; set; }
    public required DbSet<BookingStatus> BookingStatus { get; set; }
    public required DbSet<Role> Role { get; set; }
    public required DbSet<State> State { get; set; }
    public required DbSet<Product> Product { get; set; }
    public required DbSet<ProductImage> ProductImage { get; set; }
    public required DbSet<ProductUnit> ProductUnit { get; set; }
    public required DbSet<Location> Location { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
