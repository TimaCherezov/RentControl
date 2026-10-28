using Main.Model.Models;

namespace Main.Model.Repositories;

public interface IStateRepository
{
    Task<State?> GetByIdAsync(StateEnum id, CancellationToken cancellationToken = default);
}
