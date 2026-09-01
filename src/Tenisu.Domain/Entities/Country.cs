using Tenisu.Domain.Exceptions;

namespace Tenisu.Domain.Entities
{
    //Navigation property
    public record Country
    {
        /// <summary> Flag of the Country </summary>
        public Uri Picture { get; init; }
        /// <summary> Three letters code of the country </summary>
        public string Code { get; init; }

        private Country()
        {
            Picture = null!;
            Code = null!;
        }

        public Country(Uri picture, string code)
        {
            ArgumentNullException.ThrowIfNull(picture);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(code);

            if (code.Length != 3)
            {
                throw new DomainException($"The country code must have exactly 3 letters but was {code}");
            }

            Picture = picture;
            Code = code.ToUpperInvariant();
        }
    }
}
