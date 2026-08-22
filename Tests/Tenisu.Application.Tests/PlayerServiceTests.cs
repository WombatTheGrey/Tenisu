using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Tenisu.Application.DTOs;
using Tenisu.Application.Interfaces;
using Tenisu.Application.Services;
using Tenisu.Domain.Entities;
using Tenisu.Domain.Exceptions;
using Tenisu.Domain.Interfaces;

namespace Tenisu.Application.Tests
{
    [Category("Unit")]
    public class PlayerServiceTests
    {
        private IPlayerService _target;
        private Mock<ITenisuRepository> _tenisuRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CancellationToken _cancellationToken = TestContext.CurrentContext.CancellationToken;
        private readonly TenisuMapper _mapper = new TenisuMapper();

        private const string CountryCode = "FRA";
        private static readonly Country Country = new Country(new Uri("http://localhost"), CountryCode);
        private static readonly CountryDTO CountryDTO = new CountryDTO(new Uri("http://localhost"), CountryCode);
        private static readonly Data Data = new Data(15, 1234, 85, 185, 85, new List<int> { 0, 1, 0, 1, 0 });
        private static readonly DataDTO DataDTO = new DataDTO(15, 1234, 85, 185, 85, new List<int> { 0, 1, 0, 1, 0 });

        private static Player ProvidePlayer(int id) => new Player(id, "firstname", "lastname", Sex.F, Country, new Uri("http://localhost"), Data);
        private static PlayerDTO ProvidePlayerDTO() => new PlayerDTO("firstname", "lastname", Sex.F, CountryDTO, new Uri("http://localhost"), DataDTO);
        private static IEnumerable<Player> ProvidePlayers()
        {
            var player = ProvidePlayer(0);
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

        [SetUp]
        public void Setup()
        {
            var mapper = new TenisuMapper();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tenisuRepositoryMock = new Mock<ITenisuRepository>(MockBehavior.Strict);
            _target = new PlayerService(_tenisuRepositoryMock.Object, _unitOfWorkMock.Object, mapper, NullLogger<PlayerService>.Instance);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetPlayerAsync_ThowsException_WhenPlayerIdIsInvalid(int playerId)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _target.GetPlayerAsync(playerId, _cancellationToken));
        }

        [Test]
        public async Task GetPlayerAsync_ReturnsNull_WhenPlayerIsNotFound()
        {
            const int playerId = 12;
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayerAsync(playerId, _cancellationToken))
                .ReturnsAsync((Player?)null);

            var result = await _target.GetPlayerAsync(playerId, _cancellationToken);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPlayerAsync_ReturnsPlayer_WhenPlayerIsFound()
        {
            const int playerId = 12;
            var player = ProvidePlayer(playerId);
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayerAsync(playerId, _cancellationToken))
                .ReturnsAsync(player);

            var result = await _target.GetPlayerAsync(playerId, _cancellationToken);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(playerId));
            Assert.That(result.FirstName, Is.EqualTo(player.FirstName));
            Assert.That(result.LastName, Is.EqualTo(player.LastName));
        }

        [Test]
        public async Task GetAllPlayersAsync_ReturnsAllPlayersFound()
        {
            _tenisuRepositoryMock
                .Setup(repo => repo.GetAllPlayersAsync(_cancellationToken))
                .ReturnsAsync(new List<Player>());

            var firstCall = await _target.GetAllPlayersAsync(_cancellationToken);
            Assert.That(firstCall, Is.Empty);

            var players = new List<Player>() { ProvidePlayer(1) };
            _tenisuRepositoryMock
                .Setup(repo => repo.GetAllPlayersAsync(_cancellationToken))
                .ReturnsAsync(players);
            var secondCall = await _target.GetAllPlayersAsync(_cancellationToken);
            Assert.That(secondCall, Is.Not.Empty);
        }

        [TestCase(0, 15)]
        [TestCase(15, -1)]
        public void GetPageOfPlayersAsync_ThowsException_WhenPlayerIdIsInvalid(int pageNum, int pageSize)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _target.GetPageOfPlayersAsync(pageNum, pageSize, _cancellationToken));
        }

        [Test]
        public async Task GetPageOfPlayersAsync_ReturnsEmptyPage_WhenNoPlayerIsFound()
        {
            var pageNum = 1;
            var pageSize = 10;
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayersCountAsync(_cancellationToken))
                .ReturnsAsync(0);
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayersByPageAsync(pageNum, pageSize, _cancellationToken))
                .ReturnsAsync(new List<Player>());

            var result = await _target.GetPageOfPlayersAsync(pageNum, pageSize, _cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageCount, Is.Zero);
            Assert.That(result.PageNum, Is.EqualTo(pageNum));
            Assert.That(result.PageSize, Is.EqualTo(pageSize));
            Assert.That(result.Items, Is.Empty);
        }

        [Test]

        public async Task GetPageOfPlayersAsync_ReturnsPage_WhenPlayersAreFound()
        {
            var players = ProvidePlayers();
            var pageSize = 3;
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayersCountAsync(_cancellationToken))
                .ReturnsAsync(4);
            var firstPagePlayers = players.Take(3).ToList();
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayersByPageAsync(1, pageSize, _cancellationToken))
                .ReturnsAsync(firstPagePlayers);
            var secondPagePlayers = new List<Player>() { players.Last() };
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayersByPageAsync(2, pageSize, _cancellationToken))
                .ReturnsAsync(secondPagePlayers);

            var firstPage = await _target.GetPageOfPlayersAsync(1, pageSize, _cancellationToken);
            Assert.That(firstPage, Is.Not.Null);
            Assert.That(firstPage.PageNum, Is.EqualTo(1));
            Assert.That(firstPage.PageSize, Is.EqualTo(pageSize));
            Assert.That(firstPage.PageCount, Is.EqualTo(2));
            Assert.That(firstPage.Items.Select(x => x.Id), Is.EquivalentTo(firstPagePlayers.Select(x => x.Id)));

            var secondPage = await _target.GetPageOfPlayersAsync(2, pageSize, _cancellationToken);
            Assert.That(secondPage, Is.Not.Null);
            Assert.That(secondPage.PageNum, Is.EqualTo(2));
            Assert.That(secondPage.PageSize, Is.EqualTo(pageSize));
            Assert.That(secondPage.PageCount, Is.EqualTo(2));
            Assert.That(secondPage.Items.Select(x => x.Id), Is.EquivalentTo(secondPagePlayers.Select(x => x.Id)));
        }

        [Test]
        public void AddPlayerAsync_ThrowsException_WithNullPlayer()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _target.AddPlayerAsync(null!, _cancellationToken));
        }

        [Test]
        public async Task AddPlayerAsync_ThrowsException_WhenPlayerAlreadyExists()
        {
            var player = ProvidePlayer(0);
            var playerDto = ProvidePlayerDTO();
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayerAsync(player.FirstName, player.LastName, player.Sex, _cancellationToken))
                .ReturnsAsync(player);
            Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _target.AddPlayerAsync(playerDto, _cancellationToken));
        }

        [Test]
        public async Task AddPlayerAsync_ReturnsPlayerId_WhenPlayerIsAdded()
        {
            var playerDto = ProvidePlayerDTO();
            _tenisuRepositoryMock
                .Setup(repo => repo.GetPlayerAsync(playerDto.FirstName, playerDto.LastName, playerDto.Sex, _cancellationToken))
                .ReturnsAsync((Player?)null);
            _tenisuRepositoryMock
                .SetupSequence(repo => repo.GetCountryAsync(playerDto.Country.Code, _cancellationToken))
                .ReturnsAsync((Country?)null)
                .ReturnsAsync(Country);
            _tenisuRepositoryMock
                .Setup(repo => repo.AddCountryAsync(Country, _cancellationToken))
                .Returns(Task.CompletedTask);

            Player? captured = null;
            const int addedId = 12;
            _tenisuRepositoryMock
                .Setup(repo => repo.AddPlayerAsync(It.IsAny<Player>(), _cancellationToken))
                .Callback<Player, CancellationToken>((player,_)=> captured=player)
                .Returns(Task.CompletedTask);
            _unitOfWorkMock
                .Setup(uow => uow.SaveEntitiesAsync(_cancellationToken))
                .Callback<CancellationToken>(_ =>
                    {
                        typeof(Player).GetProperty(nameof(Player.Id))!.SetValue(captured, addedId);
                    })
                .Returns(Task.CompletedTask);

            var result = await _target.AddPlayerAsync(playerDto, _cancellationToken);
            Assert.That(result, Is.EqualTo(addedId));
        }
    }
}
