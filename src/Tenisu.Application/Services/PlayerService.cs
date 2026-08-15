using Microsoft.Extensions.Logging;
using Tenisu.Application.Entity;
using Tenisu.Application.Interfaces;
using Tenisu.Domain.Entities;
using Tenisu.Domain.Exceptions;
using Tenisu.Domain.Interfaces;

namespace Tenisu.Application.Services
{
    internal class PlayerService : IPlayerService
    {
        private readonly ITenisuRepository _tenisuRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(ITenisuRepository tenisuRepository, IUnitOfWork unitOfWork, ILogger<PlayerService> logger)
        {
            _tenisuRepository = tenisuRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public Task<Player?> GetPlayerAsync(int playerId, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(playerId, 1);
            return _tenisuRepository.GetPlayerAsync(playerId, cancellationToken);
        }

        public Task<IReadOnlyCollection<Player>> GetAllPlayersAsync(CancellationToken cancellationToken)
        {
            return _tenisuRepository.GetAllPlayersAsync(cancellationToken);
        }

        public async Task<Page<Player>> GetPageOfPlayersAsync(int pageNum, int pageSize, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(pageNum, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

            var playerCount = await _tenisuRepository.GetPlayersCountAsync(cancellationToken);
            var pageCount = (int)Math.Ceiling(playerCount / (double)pageSize);

            var players = await _tenisuRepository.GetPlayersByPageAsync(pageNum, pageSize, cancellationToken);

            var result = new Page<Player>(pageNum, pageSize, pageCount, players);
            return result;
        }

        public async Task<int> AddPlayerAsync(Player player, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(player);
            if (player.Id != 0)
            {
                throw new ArgumentException("A new player must have Id = 0.", nameof(player));
            }

            _logger.LogInformation("Trying to add a new player '{player}'", player.ShortName);

            var retrievedPlayer = await _tenisuRepository.GetPlayerAsync(player.FirstName, player.LastName, player.Sex, cancellationToken); 
            if(retrievedPlayer is not null)
            {
                _logger.LogInformation("The player '{player}' already exists and will not be added", player.ShortName);
                throw new EntityAlreadyExistsException("Player already exists");
            }

            var savedCountry = await _tenisuRepository.GetCountryAsync(player.Country.Code, cancellationToken);
            if (savedCountry is null)
            {
                _logger.LogInformation("The country '{countryCode}' could not be retrieved. It will be added first.", player.Country.Code);
                await _tenisuRepository.AddCountryAsync(player.Country, cancellationToken);
            }

            await _tenisuRepository.AddPlayerAsync(player, cancellationToken);
            await _unitOfWork.SaveEntitiesAsync(cancellationToken);

            return player.Id;            
        }
    }
}
