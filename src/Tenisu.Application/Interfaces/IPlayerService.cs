using Tenisu.Domain.Entities;

namespace Tenisu.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<IReadOnlyList<Player>> GetAllPlayersAsync(CancellationToken cancellationToken);
        Task<Player> GetPlayerAsync(int playerId, CancellationToken cancellationToken);
        Task AddPlayer(Player player, CancellationToken cancellationToken);
    }
}
