using Tenisu.Domain.Entities;

namespace Tenisu.Domain.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<Country> GetMostSuccesfullCountryAsync(CancellationToken cancellationToken);
        Task<double> GetAverageIMCAsync(CancellationToken cancellationToken);
        Task<double> GetMedianPlayerHeight(CancellationToken cancellationToken);
    }
}
