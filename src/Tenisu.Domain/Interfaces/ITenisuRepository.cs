using Tenisu.Domain.Entities;

namespace Tenisu.Domain.Interfaces
{
    public interface ITenisuRepository
    {
        ValueTask<Player?> GetPlayerByIdAsync(int playerId, CancellationToken cancellationToken);
        Task<int> GetPlayersCountAsync(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Player>> GetPagedPlayersAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Player>> GetAllPlayersAsync(CancellationToken cancellationToken);
        Task AddPlayerAsync(Player player, CancellationToken cancellationToken);

        ValueTask<Country?> GetCountryAsync(string countryCode, CancellationToken cancellationToken);
        Task AddCountryAsync(Country country, CancellationToken cancellationToken);
    }
}
