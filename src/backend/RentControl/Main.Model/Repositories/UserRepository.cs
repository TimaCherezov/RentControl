using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class UserRepository(MainDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<List<User>> GetTenantsAsync(CancellationToken cancellationToken = default)
        => dbContext.Users
            .Where(u => dbContext.Bookings.Any(b => b.UserId == u.Id))
            .ToListAsync(cancellationToken);

    public void Add(User user)
        => dbContext.Users.Add(user);

    public void Update(User user)
        => dbContext.Users.Update(user);
}
