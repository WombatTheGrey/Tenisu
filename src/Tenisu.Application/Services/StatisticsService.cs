using Tenisu.Application.DTOs;
using Tenisu.Application.Interfaces;
using Tenisu.Domain.Interfaces;

namespace Tenisu.Application.Services
{
    internal class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly TenisuMapper _mapper;

        public StatisticsService(IStatisticsRepository statisticsRepository, TenisuMapper mapper)
        {
            _statisticsRepository = statisticsRepository;
            _mapper = mapper;
        }

        public Task<double> GetAverageIMCAsync(CancellationToken cancellationToken) => _statisticsRepository.GetAverageIMCAsync(cancellationToken);
        public Task<double> GetMedianPlayerHeightAsync(CancellationToken cancellationToken) => _statisticsRepository.GetMedianPlayerHeight(cancellationToken);
        public async Task<CountryDTO> GetMostSuccessfulCountryAsync(CancellationToken cancellationToken)
        {
            var country = await _statisticsRepository.GetMostSuccessfulCountryAsync(cancellationToken);
            return _mapper.ToDTO(country);
        }
    }
}
