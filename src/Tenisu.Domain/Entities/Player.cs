using Tenisu.Domain.Exceptions;

namespace Tenisu.Domain.Entities
{
    public record Player
    {
        /// <summary> Unique Id number of the player. A value will be provided for you. </summary>
        public int Id { get; init; }
        /// <summary> The player's first name </summary>
        public string FirstName { get; init; }
        /// <summary>The player's last name </summary>
        public string LastName { get; init; }
        /// <summary> A abreviation of the player's full name </summary>
        /// <example>N.DJO</example>
        public string ShortName => GetShortName();
        /// <summary> The player's sex </summary>
        public Sex Sex { get; init; }
        /// <summary>The player's Country </summary>
        public Country Country { get; init; }//Navigation property
        /// <summary>The player's Picture </summary>
        public Uri Picture { get; init; }
        /// <summary>The player's Data </summary>
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
