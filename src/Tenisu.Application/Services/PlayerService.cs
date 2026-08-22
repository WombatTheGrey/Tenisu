using Microsoft.Extensions.Logging;
using Tenisu.Application.DTOs;
using Tenisu.Application.Interfaces;
using Tenisu.Application.Model;
using Tenisu.Domain.Entities;
using Tenisu.Domain.Exceptions;
using Tenisu.Domain.Interfaces;

namespace Tenisu.Application.Services
{
    internal class PlayerService : IPlayerService
    {
        private readonly ITenisuRepository _tenisuRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TenisuMapper _mapper;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(ITenisuRepository tenisuRepository, IUnitOfWork unitOfWork, TenisuMapper mapper, ILogger<PlayerService> logger)
        {
            _tenisuRepository = tenisuRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PlayerResponseDTO?> GetPlayerAsync(int playerId, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(playerId, 1);
            var player = await _tenisuRepository.GetPlayerAsync(playerId, cancellationToken);
            return player is null ? null : _mapper.ToDTO(player);
        }

        public async Task<IReadOnlyCollection<PlayerResponseDTO>> GetAllPlayersAsync(CancellationToken cancellationToken)
        {
            var players = await _tenisuRepository.GetAllPlayersAsync(cancellationToken);
            return _mapper.ToDTO(players);
        }

        public async Task<Page<PlayerResponseDTO>> GetPageOfPlayersAsync(int pageNum, int pageSize, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(pageNum, 1);
            ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

            var playerCount = await _tenisuRepository.GetPlayersCountAsync(cancellationToken);
            var pageCount = (int)Math.Ceiling(playerCount / (double)pageSize);

            var players = await _tenisuRepository.GetPlayersByPageAsync(pageNum, pageSize, cancellationToken);
            var mappedPlayers = _mapper.ToDTO(players);
            var result = new Page<PlayerResponseDTO>(pageNum, pageSize, pageCount, mappedPlayers);
            return result;
        }

        public async Task<int> AddPlayerAsync(PlayerDTO playerDto, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(playerDto);

            _logger.LogInformation("Trying to add a new player '{player}'", playerDto.FirstName+playerDto.LastName);//log pattern ??

            var retrievedPlayer = await _tenisuRepository.GetPlayerAsync(playerDto.FirstName, playerDto.LastName, playerDto.Sex, cancellationToken); 
            if(retrievedPlayer is not null)
            {
                _logger.LogInformation("The player '{player}' already exists and will not be added", playerDto.FirstName + playerDto.LastName);
                throw new EntityAlreadyExistsException("Player already exists");
            }

            var country = _mapper.ToDomain(playerDto.Country);
            var data = _mapper.ToDomain(playerDto.Data);
            var savedCountry = await _tenisuRepository.GetCountryAsync(country.Code, cancellationToken);
            if (savedCountry is null)
            {
                _logger.LogInformation("The country '{countryCode}' could not be retrieved. It will be added first.", playerDto.Country.Code);
                await _tenisuRepository.AddCountryAsync(country, cancellationToken);
            }

            var player = new Player(0, playerDto.FirstName, playerDto.LastName, playerDto.Sex,
                country, playerDto.Picture, data);

            await _tenisuRepository.AddPlayerAsync(player, cancellationToken);
            await _unitOfWork.SaveEntitiesAsync(cancellationToken);

            return player.Id;            
        }
    }
}
