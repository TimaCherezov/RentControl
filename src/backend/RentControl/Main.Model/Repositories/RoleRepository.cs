using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;


public class RoleRepository(MainDbContext dbcontext) : IRoleRepository
{
    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        => await dbcontext.Role.AddAsync(role);

    public async Task DeleteByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var role = await dbcontext.Role.FirstOrDefaultAsync(r => r.Code == code);
        if (role == null)
            return;
        dbcontext.Role.Remove(role);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await dbcontext.Role.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
            return;
        dbcontext.Role.Remove(role);
    }

    public async Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await dbcontext.Role.FirstOrDefaultAsync(r => r.Code == code);

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbcontext.Role.FirstOrDefaultAsync(r => r.Id == id);
}
