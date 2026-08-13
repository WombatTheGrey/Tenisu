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

        public Task<Player?> GetPlayerAsync(int playerId, CancellationToken cancellationToken)
        {
            return _dbContext.Players.FindAsync([playerId], cancellationToken: cancellationToken).AsTask();
        }

        public Task<Player?> GetPlayerAsync(string firstName, string lastName, Sex sex, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
            return _dbContext.Players
                .AsNoTracking()
                .Include(p => p.Country)
                .Where(p => p.FirstName == firstName
                    && p.LastName == lastName
                    && p.Sex == sex)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Player>> GetAllPlayersAsync(CancellationToken cancellationToken)
        {
            var players = await _dbContext.Players
                .AsNoTracking()
                .Include(x => x.Country)
                .ToListAsync(cancellationToken);
            return players.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Player>> GetPlayersByPageAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

            var playersQuery = _dbContext.Players
                .AsNoTracking()
                .Include(x => x.Country)
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
