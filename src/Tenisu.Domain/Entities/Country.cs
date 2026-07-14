namespace Tenisu.Domain.Entities
{
    public record Country
    {
        public Uri Picture { get; private set; }
        public string Code { get; private set; }

        private Country()
        {
            Picture = null!;
            Code = null!;
        }

        public Country(Uri picture, string code)
        {
            Picture = picture;
            Code = code;
        }
    }
}
