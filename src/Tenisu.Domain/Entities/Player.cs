using Tenisu.Domain.Exceptions;

namespace Tenisu.Domain.Entities
{
    public record Player
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private string? _shortName;
        public string ShortName => _shortName ??= GetShortName();
        public Sex Sex { get; private set; }
        public Country Country { get; private set; }
        public Uri Picture { get; private set; }
        public Data Data { get; private set; }

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
                throw new InvalidPlayerStateException("FirstName and LastName must not be null or whitespace to generate ShortName.");
            }

            return $"{FirstName[0]}.{LastName.Substring(Math.Min(3, LastName.Length))}";
        }
    }
}
