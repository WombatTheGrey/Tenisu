using Tenisu.Domain.Entities;

namespace Tenisu.Domain.Interfaces
{
    public interface ITenisuRepository
    {
        Task<Player?> GetPlayerAsync(int playerId, CancellationToken cancellationToken);
        Task<Player?> GetPlayerAsync(string firstName, string lastName, Sex sex, CancellationToken cancellationToken);

        Task<int> GetPlayersCountAsync(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Player>> GetPlayersByPageAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Player>> GetAllPlayersAsync(CancellationToken cancellationToken);
        Task AddPlayerAsync(Player player, CancellationToken cancellationToken);

        ValueTask<Country?> GetCountryAsync(string countryCode, CancellationToken cancellationToken);
        Task AddCountryAsync(Country country, CancellationToken cancellationToken);
    }
}
