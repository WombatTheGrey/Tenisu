using Tenisu.Domain.Exceptions;

namespace Tenisu.Domain.Entities
{
    public record Player
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }

        private string? _shortName;
        public string ShortName => _shortName ??= GetShortName();
        public Sex Sex { get; init; }
        public Country Country { get; init; }//Navigation property
        public Uri Picture { get; init; }
        public Data Data { get; init; }//Owned type

        private Player()
        {
            Id = 0;
            FirstName = null!;
            LastName = null!;
            Sex = Sex.Undefined;
            Country = null!;
            Picture = null!;
            Data = null!;
        }

        public Player(int id, string firstName, string lastName, Sex sex, Country country, Uri picture, Data data)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(id, 0);
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
            ArgumentNullException.ThrowIfNull(country);
            ArgumentNullException.ThrowIfNull(picture);
            ArgumentNullException.ThrowIfNull(data);
            if(sex is Sex.Undefined)
            {
                throw new DomainException($"sex must have a value different from {Sex.Undefined}");
            }

            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Sex = sex;
            Country = country;
            Picture = picture;
            Data = data;
        }

        private string GetShortName()
        {
            if(string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                throw new InvalidOperationException("FirstName and LastName must not be null or whitespace to generate ShortName.");
            }

            return $"{FirstName[0]}.{LastName.Substring(0, Math.Min(3, LastName.Length))}".ToUpperInvariant();
        }
    }
}
