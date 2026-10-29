using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

public class StateRepository(MainDbContext dbContext) : IStateRepository
{
    public Task<State?> GetByIdAsync(StateEnum id, CancellationToken cancellationToken = default) =>
        dbContext.States.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}
