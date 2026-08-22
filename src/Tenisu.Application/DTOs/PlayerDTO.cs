using Tenisu.Domain.Entities;

namespace Tenisu.Application.DTOs
{
    public record PlayerDTO
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public Sex Sex { get; init; }
        public CountryDTO Country { get; init; }
        public Uri Picture { get; init; }
        public DataDTO Data { get; init; }

        public PlayerDTO(string firstName, string lastName, Sex sex, CountryDTO country, Uri picture, DataDTO data)
        {
            FirstName = firstName;
            LastName = lastName;
            Sex = sex;
            Country = country;
            Picture = picture;
            Data = data;
        }
    }
}
