using Microsoft.AspNetCore.Mvc;
using Tenisu.Application.DTOs;
using Tenisu.Application.Interfaces;

namespace Tenisu.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("AverageIMC")]
        [ProducesResponseType<double>(StatusCodes.Status200OK)]
        public async Task<ActionResult<double>> GetAverageIMC(CancellationToken cancellationToken)
        {
            var imc = await _statisticsService.GetAverageIMCAsync(cancellationToken);
            return Ok(imc);
        }

        [HttpGet("MedianHeight")]
        [ProducesResponseType<double>(StatusCodes.Status200OK)]
        public async Task<ActionResult<double>> GetMedianPlayerHeight(CancellationToken cancellationToken)
        {
            var median = await _statisticsService.GetMedianPlayerHeightAsync(cancellationToken);
            return Ok(median);
        }

        [HttpGet("MostSuccessfulCountry")]
        [ProducesResponseType<CountryDTO>(StatusCodes.Status200OK)]
        public async Task<ActionResult<CountryDTO>> GetMostSuccessfulCountry(CancellationToken cancellationToken)
        {
            var country = await _statisticsService.GetMostSuccessfulCountryAsync(cancellationToken);
            return Ok(country);
        }
    }
}
