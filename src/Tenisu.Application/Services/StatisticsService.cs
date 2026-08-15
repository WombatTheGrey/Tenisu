using Tenisu.Application.Interfaces;
using Tenisu.Domain.Interfaces;

namespace Tenisu.Application.Services
{
    internal class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public Task GetAverageIMCAsync(CancellationToken cancellationToken) => _statisticsRepository.GetAverageIMCAsync(cancellationToken);
        public Task GetMedianPlayerHeightAsync(CancellationToken cancellationToken) => _statisticsRepository.GetMedianPlayerHeight(cancellationToken);
        public Task GetMostSuccesfullCountryAsync(CancellationToken cancellationToken) => _statisticsRepository.GetMostSuccesfullCountryAsync(cancellationToken);
    }
}
