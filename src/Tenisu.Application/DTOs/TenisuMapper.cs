using Riok.Mapperly.Abstractions;
using Tenisu.Domain.Entities;

namespace Tenisu.Application.DTOs
{
    [Mapper]
    public partial class TenisuMapper
    {
        //DTO to Domain
        [MapProperty(nameof(DataDTO.WeightInGrams), nameof(Data.Weight))]
        [MapProperty(nameof(DataDTO.HeightInCm), nameof(Data.Height))]
        public partial Data ToDomain(DataDTO dataDTO);
        public partial Country ToDomain(CountryDTO countryDTO);

        [MapValue(nameof(Player.Id), 0)]
        public partial Player ToDomain(PlayerDTO playerDTO);

        //Domain to DTO
        [MapProperty(nameof(Data.Weight), nameof(DataDTO.WeightInGrams))]
        [MapProperty(nameof(Data.Height), nameof(DataDTO.HeightInCm))]
        public partial DataDTO ToDTO(Data data);
        public partial CountryDTO ToDTO(Country country);

        [MapperIgnoreSource(nameof(Player.ShortName))]
        public partial PlayerResponseDTO ToDTO(Player player);

        public partial IReadOnlyCollection<PlayerResponseDTO> ToDTO(IReadOnlyCollection<Player> players);
    }
}
