using System;
using System.Collections.Generic;
using Tenisu.Application.Interfaces;

namespace Tenisu.Application.Services
{
    internal class StatisticsService : IStatisticsService
    {
        public Task GetAverageIMCAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task GetMedianPlayerHeight(CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task GetMostSuccesfullCountryAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
