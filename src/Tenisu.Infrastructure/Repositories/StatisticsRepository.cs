using Microsoft.EntityFrameworkCore;
using Tenisu.Domain.Entities;
using Tenisu.Domain.Interfaces;
using Tenisu.Infrastructure.Context;

namespace Tenisu.Infrastructure.Repositories
{
    internal class StatisticsRepository : IStatisticsRepository
    {
        private readonly TenisuDbContext _dbContext;

        public StatisticsRepository(TenisuDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<double> GetAverageIMCAsync(CancellationToken cancellationToken)
        {
            var average = await _dbContext.Players.AsNoTracking()
                .AverageAsync(p => p.Data.Weight * 10 / Math.Pow(p.Data.Height, 2), cancellationToken);

            return Math.Round(average, 4);
        }

        public async Task<double> GetMedianPlayerHeight(CancellationToken cancellationToken)
        {
            var count = await _dbContext.Players.CountAsync(cancellationToken);

            var median = await _dbContext.Players.AsNoTracking()
                .OrderBy(p => p.Data.Height)
                .Skip((count - 1) / 2)
                .Take(2 - (count % 2))
                .AverageAsync(p => p.Data.Height, cancellationToken);

            return Math.Round(median, 4);
        }

        public async Task<Country> GetMostSuccesfullCountryAsync(CancellationToken cancellationToken)
        {
            var groups = await _dbContext.Players.AsNoTracking()
                .GroupBy(p => p.Country)
                .ToListAsync(cancellationToken);

            var country = groups
                .Select(g => new
                {
                    Country = g.Key,
                    Ratio = g.SelectMany(p => p.Data.Last).Average()
                })
                .OrderByDescending(x => x.Ratio)
                .First()
                .Country;

            return country;
        }
    }
}
