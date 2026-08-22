using Tenisu.Application.DTOs;
using Tenisu.Application.Model;

namespace Tenisu.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerResponseDTO?> GetPlayerAsync(int playerId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<PlayerResponseDTO>> GetAllPlayersAsync(CancellationToken cancellationToken);
        Task<Page<PlayerResponseDTO>> GetPageOfPlayersAsync(int pageNum, int pageSize, CancellationToken cancellationToken);
        Task<int> AddPlayerAsync(PlayerDTO playerDto, CancellationToken cancellationToken);
    }
}
