using Tenisu.Application.DTOs;

namespace Tenisu.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<CountryDTO> GetMostSuccessfulCountryAsync(CancellationToken cancellationToken);
        Task<double> GetAverageIMCAsync(CancellationToken cancellationToken);
        Task<double> GetMedianPlayerHeightAsync(CancellationToken cancellationToken);
    }
}
