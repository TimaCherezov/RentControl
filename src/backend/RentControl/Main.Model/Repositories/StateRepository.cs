using Main.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Model.Repositories;

class StateRepository(MainDbContext dbcontext) : IStateRepository
{
    public Task<State?> GetByIdAsync(StateEnum id, CancellationToken cancellationToken = default) =>
        dbcontext.States.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}
