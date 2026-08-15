using Tenisu.Application.Entity;
using Tenisu.Domain.Entities;

namespace Tenisu.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<Player?> GetPlayerAsync(int playerId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Player>> GetAllPlayersAsync(CancellationToken cancellationToken);
        Task<Page<Player>> GetPageOfPlayersAsync(int pageNum, int pageSize, CancellationToken cancellationToken);
        Task<int> AddPlayerAsync(Player player, CancellationToken cancellationToken);
    }
}
