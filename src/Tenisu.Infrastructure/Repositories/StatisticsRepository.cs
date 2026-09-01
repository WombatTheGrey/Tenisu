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

        public async Task<double> GetMedianPlayerHeightAsync(CancellationToken cancellationToken)
        {
            var count = await _dbContext.Players.AsNoTracking().CountAsync(cancellationToken);

            var median = await _dbContext.Players.AsNoTracking()
                .OrderBy(p => p.Data.Height)
                .Skip((count - 1) / 2)
                .Take(2 - (count % 2))
                .AverageAsync(p => p.Data.Height, cancellationToken);

            return Math.Round(median, 4);
        }

        //Note : This implementation loads all the players in memory. I choose to live it like this because
        //SQLite can't deal with collection queries accross rows "g.SelectMany(p => p.Data.Last).Average()" breaks;
        //The best implementation would of course be streaming and computing on the go. 
        public async Task<Country> GetMostSuccessfulCountryAsync(CancellationToken cancellationToken)
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
