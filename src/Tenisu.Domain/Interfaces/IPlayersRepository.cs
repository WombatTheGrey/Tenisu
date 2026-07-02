using Tenisu.Domain.Entities;

namespace Tenisu.Domain.Interfaces
{
    public interface IPlayersRepository
    {
        ValueTask<Player> GetPlayerAsync(int id);
        ValueTask<IEnumerable<Player>> GetAllPlayersAsync();
        ValueTask AddPlayersAsync(IEnumerable<Player> player);
    }
}
