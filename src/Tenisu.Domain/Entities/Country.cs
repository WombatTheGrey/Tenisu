namespace Tenisu.Domain
{
    public record Country
    {
        public Uri Picture { get; init; }
        public string Code { get; init; }

        public Country(Uri picture, string code)
        {
            ArgumentNullException.ThrowIfNull(picture);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(code);

            if (code.Length > 3)
            {
                throw new domain
            }

            Picture = picture;
            Code = code;
        }
    }
}
