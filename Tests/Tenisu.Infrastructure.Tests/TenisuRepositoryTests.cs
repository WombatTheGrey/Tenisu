using Microsoft.EntityFrameworkCore;
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
        private static readonly Country Country = new Country(new Uri("http://localhost"), CountryCode);
        private static readonly Data Data = new Data(15, 1234, 85, 185, 85,new List<int> { 0, 1, 0, 1, 0 });

        [SetUp]
        public void SetUp()
        {
            _target = new TenisuRepository(DbContext);
        }

        #region Country
        [Test]
        public async Task Should_ReturnNull_When_CountryIsNotFound()
        {
            var failedRetrieval = await _target.GetCountryAsync(CountryCode, CancellationToken);
            Assert.That(failedRetrieval, Is.Null);
        }

        [Test]
        public async Task Should_ReturnCountry_When_CountryIsFound()
        {
            await _target.AddCountryAsync(Country, CancellationToken);
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);
            var retrieved = await _target.GetCountryAsync(CountryCode, CancellationToken);
            Assert.That(retrieved, Is.Not.Null.And.EqualTo(Country));
        }
        #endregion Country

        #region Player
        private static Player ProvidePlayer() => new Player(0, "firstname", "lastname", Sex.F, Country, new Uri("http://localhost"), Data);
        private static IEnumerable<Player> ProvidePlayers()
        {
            var player = ProvidePlayer();
            yield return player with
            {
                Data = Data with { Rank = 1 },
                FirstName = "p1"
            };
            yield return player with
            {
                Data = Data with { Rank = 2 },
                FirstName = "p2"
            };
            yield return player with
            {
                Data = Data with { Rank = 3 },
                FirstName = "p3"
            };
            yield return player with
            {
                Data = Data with { Rank = 4 },
                FirstName = "p4"
            };
        }

        [Test]
        public async Task Should_ReturnNull_When_PlayerIdIsNotFound()
        {
            var failedRetrieval = await _target.GetPlayerAsync(12, CancellationToken);
            Assert.That(failedRetrieval, Is.Null);
        }

        [Test]
        public async Task Should_ReturnNull_When_PlayerInfoIsNotFound()
        {
            var failedRetrieval = await _target.GetPlayerAsync("wrong", "name", Sex.F, CancellationToken);
            Assert.That(failedRetrieval, Is.Null);
        }

        [TestCase("", "value")]
        [TestCase("  ", "value")]
        [TestCase("value", "")]
        [TestCase("value", "  ")]
        public async Task Should_ThrowAnException_When_InvalidInputProvided(string? firstName, string? lastName)
        {
            Assert.ThrowsAsync<ArgumentException>(() => _target.GetPlayerAsync(firstName!, lastName!, Sex.F, CancellationToken));
        }

        [Test]
        public async Task Should_FailToAddPlayer_When_AlreadyExists()
        {
            var player = ProvidePlayer();
            await _target.AddPlayerAsync(player, CancellationToken);
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);

            await _target.AddPlayerAsync(player, CancellationToken);
            Assert.ThrowsAsync<DbUpdateException>(() => this.UnitOfWork.SaveEntitiesAsync(CancellationToken));
        }

        [Test]
        public async Task Should_AddPlayer_When_PlayerIsValid()
        {
            var player = ProvidePlayer();
            await _target.AddPlayerAsync(player, CancellationToken);
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);
            Assert.That(player.Id, Is.Not.Zero);
        }

        [Test]
        public async Task Should_ReturnPlayer_When_PlayerIdIsFound()
        {
            var player = ProvidePlayer();
            await _target.AddPlayerAsync(player, CancellationToken);
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);
            var retrieved = await _target.GetPlayerAsync(player.Id, CancellationToken);
            Assert.That(retrieved, Is.Not.Null.And.EqualTo(player));
        }

        [Test]
        public async Task Should_ReturnPlayer_When_PlayerInfoIsFound()
        {
            var player = ProvidePlayer();
            await _target.AddPlayerAsync(player, CancellationToken);
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);
            var retrieved = await _target.GetPlayerAsync(player.FirstName, player.LastName, player.Sex, CancellationToken);
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved.FirstName, Is.EqualTo(player.FirstName));
            Assert.That(retrieved.LastName, Is.EqualTo(player.LastName));
            Assert.That(retrieved.Sex, Is.EqualTo(player.Sex));
            Assert.That(retrieved.Country, Is.EqualTo(player.Country));
            Assert.That(retrieved.Picture, Is.EqualTo(player.Picture));
        }

        [Test]
        public async Task Should_ReturnCount_InEveryCase()
        {
            var firstCount = await _target.GetPlayersCountAsync(CancellationToken);
            Assert.That(firstCount, Is.Zero);

            var player = ProvidePlayer();
            await _target.AddPlayerAsync(player, CancellationToken);
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);

            var secondCount = await _target.GetPlayersCountAsync(CancellationToken);
            Assert.That(secondCount, Is.EqualTo(1));
        }

        [Test]
        public async Task Should_Return_EmptyCollection_When_NoPlayersAdded()
        {
            var players = await _target.GetAllPlayersAsync(CancellationToken);
            Assert.That(players, Is.Not.Null.And.Empty);
        }

        [Test]
        public async Task Should_Return_EmptyPagedCollection_When_NoPlayersAdded()
        {
            var players = await _target.GetPlayersByPageAsync(1, 25, CancellationToken);
            Assert.That(players, Is.Not.Null.And.Empty);
        }

        [Test]
        public async Task Should_Return_EmptyPagedCollection_When_PageIsEmpty()
        {
            var player = ProvidePlayer();
            await _target.AddPlayerAsync(player, CancellationToken);
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);
            var players = await _target.GetPlayersByPageAsync(2, 25, CancellationToken);
            Assert.That(players, Is.Not.Null.And.Empty);
        }

        [TestCase(-1, 10)]
        [TestCase(1, -1)]
        public async Task Should_ThrowArgumentException_When_InvalidParameters(int page, int pageSize)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _target.GetPlayersByPageAsync(page, pageSize, CancellationToken));
        }

        [Test]
        public async Task Should_Return_AllPlayers_When_PlayersAdded()
        {
            var providedPlayers = ProvidePlayers().ToList();

            foreach (var player in providedPlayers)
            {
                await _target.AddPlayerAsync(player, CancellationToken);
            }
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);

            var players = await _target.GetAllPlayersAsync(CancellationToken);
            Assert.That(players.Select(p => p.Id), Is.EquivalentTo(providedPlayers.Select(p => p.Id)));
        }

        [Test]
        public async Task Should_Return_AllPagedPlayers_When_PlayersAdded()
        {
            var providedPlayers = ProvidePlayers().ToList();
            var providedPlayer = ProvidePlayer();
            foreach (var player in providedPlayers)
            {
                await _target.AddPlayerAsync(player, CancellationToken);
            }
            await _target.AddPlayerAsync(providedPlayer, CancellationToken);
            await this.UnitOfWork.SaveEntitiesAsync(CancellationToken);

            var firstPage = await _target.GetPlayersByPageAsync(1, 4, CancellationToken);
            Assert.That(firstPage.Select(p => p.Id), Is.EquivalentTo(providedPlayers.Select(p => p.Id)));

            var secondPage = await _target.GetPlayersByPageAsync(2, 4, CancellationToken);
            Assert.That(secondPage.Select(p => p.Id), Is.EquivalentTo([providedPlayer.Id]));
        }

        #endregion
    }
}
