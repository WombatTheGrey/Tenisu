using Tenisu.Domain.Exceptions;

namespace Tenisu.Domain.Entities
{
    //Navigation property
    public record Country
    {
        public Uri Picture { get; init; }
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
                throw new DomainException("The country code must have exactly 3 letters");
            }

            Picture = picture;
            Code = code;
        }
    }
}
