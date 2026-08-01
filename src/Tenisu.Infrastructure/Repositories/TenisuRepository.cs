using Microsoft.EntityFrameworkCore;
using Tenisu.Domain.Entities;
using Tenisu.Domain.Interfaces;
using Tenisu.Infrastructure.Context;

namespace Tenisu.Infrastructure.Repositories
{
    internal class TenisuRepository : ITenisuRepository
    {
        private readonly TenisuDbContext _dbContext;

        public TenisuRepository(TenisuDbContext context)
        {
            _dbContext = context;
        }

        public ValueTask<Player?> GetPlayerByIdAsync(int playerId, CancellationToken cancellationToken)
        {
            return _dbContext.Players.FindAsync([playerId], cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyCollection<Player>> GetAllPlayersAsync(CancellationToken cancellationToken)
        {
            var players = await _dbContext.Players
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return players.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Player>> GetPagedPlayersAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(1, page);
            ArgumentOutOfRangeException.ThrowIfLessThan(25, pageSize);

            var playersQuery = _dbContext.Players.AsNoTracking()
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return await playersQuery.ToListAsync(cancellationToken);
        }

        public Task<int> GetPlayersCountAsync(CancellationToken cancellationToken)
        {
            return _dbContext.Players.CountAsync(cancellationToken);
        }

        public async Task AddPlayerAsync(Player player, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(player, cancellationToken);
        }

        public ValueTask<Country?> GetCountryAsync(string countryCode, CancellationToken cancellationToken)
        {
            return _dbContext.Countries.FindAsync([countryCode], cancellationToken: cancellationToken);
        }

        public async Task AddCountryAsync(Country country, CancellationToken cancellationToken)
        {
            await _dbContext.Countries.AddAsync(country, cancellationToken);
        }
    }
}
