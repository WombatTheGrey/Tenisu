using NUnit.Framework;
using Tenisu.Domain.Entities;
using Tenisu.Domain.Interfaces;
using Tenisu.Infrastructure.Repositories;

namespace Tenisu.Infrastructure.Tests
{
    public class StatisticsRepositoryTests : BaseRepositoryTests
    {
        private IStatisticsRepository _target;

        [SetUp]
        public async Task SetUp()
        {
            _target = new StatisticsRepository(DbContext);
            var players = ProvidePlayers();
            await DbContext.Players.AddRangeAsync(players, CancellationToken);
            await DbContext.SaveChangesAsync(CancellationToken);
        }

        private static IEnumerable<Player> ProvidePlayers()
        {
            var p1Data = new Data(6, 123, 60000, 160, 26, [1, 0, 1, 0, 1]);
            yield return new Player(0, "p1", "p1", Sex.M, Country, new Uri("http://localhost"), p1Data);

            var p2Data = new Data(7, 123, 70000, 170, 27, [1, 0, 1, 0, 1]);
            yield return new Player(0, "p2", "p2", Sex.M, Country, new Uri("http://localhost"), p2Data);

            var secondCountry = new Country(new Uri("http://localhost"), "ENG");
            var p3Data = new Data(8, 123, 80000, 180, 28, [0, 1, 0, 1, 0]);
            yield return new Player(0, "p3", "p3", Sex.M, secondCountry, new Uri("http://localhost"), p3Data);

            var p4Data = new Data(9, 123, 90000, 190, 29, [0, 1, 0, 1, 0]);
            yield return new Player(0, "p4", "p4", Sex.M, secondCountry, new Uri("http://localhost"), p4Data);
        }

        [Test]
        public async Task GetAverageIMCAsync_ReturnsExceptedValue()
        {
            var result = await _target.GetAverageIMCAsync(CancellationToken);
            Assert.That(Math.Abs(result-24.320), Is.LessThanOrEqualTo(0.001));
        }

        [Test]
        public async Task GetMedianPlayerHeight_ReturnsExpectedValue()
        {
            var result = await _target.GetMedianPlayerHeight(CancellationToken);
            Assert.That(result, Is.EqualTo(175));

            var p5Data = new Data(5, 123, 50000, 150, 25, [0, 1, 0, 1, 0]);
            var newPlayer = new Player(0, "p5", "p5", Sex.M, Country, new Uri("http://localhost"), p5Data);
            await DbContext.Players.AddAsync(newPlayer, CancellationToken);
            await DbContext.SaveChangesAsync(CancellationToken);

            result = await _target.GetMedianPlayerHeight(CancellationToken);
            Assert.That(result, Is.EqualTo(170));
        }

        [Test]
        public async Task GetMostSuccesfullCountryAsync_ReturnsExpectedValue()
        {
            var result = await _target.GetMostSuccesfullCountryAsync(CancellationToken);
            Assert.That(result, Is.EqualTo(Country));
        }
    }
}
