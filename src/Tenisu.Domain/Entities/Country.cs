namespace Tenisu.Domain.Entities
{
    public record Country
    {
        public required Uri Picture { get; init; }
        public required string Code { get; init; }
    }
}
