namespace Tenisu.Application.DTOs
{
    public record CountryDTO
    {
        public Uri Picture { get; init; }
        public string Code { get; init; }

        public CountryDTO(Uri picture, string code)
        {
            Picture = picture;
            Code = code;
        }
    }
}
