using Tenisu.Domain.Entities;

namespace Tenisu.Application.DTOs
{
    public record PlayerResponseDTO : PlayerDTO
    {
        public int Id { get; init; }
        public PlayerResponseDTO(int id, string firstName, string lastName, Sex sex, CountryDTO country, Uri picture, DataDTO data) :
            base(firstName, lastName, sex, country, picture, data)
        {
            Id = id;
        }
    }
}
