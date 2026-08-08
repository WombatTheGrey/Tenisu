using NUnit.Framework;
using Tenisu.Domain.Entities;
using Tenisu.Domain.Interfaces;
using Tenisu.Infrastructure.Repositories;

namespace Tenisu.Infrastructure.Tests
{
    [Category("Integration")]
    public class TenisuRepositoryTests : BaseRepositoryTests
    {
        private ITenisuRepository _target;

        private const string CountryCode = "FRA";
        private readonly Country _country = new Country(new Uri("http://localhost"), CountryCode);

        [SetUp]
        public void SetUp()
        {
            _target = new TenisuRepository(DbContext);
        }

        [Test]
        public async Task Should_ReturnCountry_When_PreviouslyAdded()
        {
            var failedRetrieval = await _target.GetCountryAsync(CountryCode, CancellationToken);
            Assert.That(failedRetrieval, Is.Null);

            var succesfulRetrieval = await _target.GetCountryAsync(CountryCode, CancellationToken);
            Assert.That(succesfulRetrieval, Is.Not.Null.And.EqualTo(_country));
        }

        [Test]
        public async Task Should_FailToAddPlayer_When_IdHasAValue()
        {
            var playerData = new Data(15, 1234, 85, 185, 85, null);

            //var player = new Player(12, "firstname", "lastname", Sex.F, _country, new Uri("http://localhost"))
        }

        [Test]
        public async Task Should_ReturnPlayer_When_PreviouslyAdded()
        {
            //var player = new Player()
        }
    }
}
