namespace Tenisu.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task GetMostSuccesfullCountryAsync(CancellationToken cancellationToken);
        Task GetAverageIMCAsync(CancellationToken cancellationToken);
        Task GetMedianPlayerHeight(CancellationToken cancellationToken);
    }
}
