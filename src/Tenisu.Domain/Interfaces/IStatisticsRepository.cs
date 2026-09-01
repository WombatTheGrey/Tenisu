using Tenisu.Domain.Entities;

namespace Tenisu.Domain.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<Country> GetMostSuccessfulCountryAsync(CancellationToken cancellationToken);
        Task<double> GetAverageIMCAsync(CancellationToken cancellationToken);
        Task<double> GetMedianPlayerHeightAsync(CancellationToken cancellationToken);
    }
}
