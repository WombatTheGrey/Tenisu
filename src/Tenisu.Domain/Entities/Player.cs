namespace Tenisu.Domain.Entities
{
    public record Player
    {
        public int Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string ShortName { get; init; }
        public required Sex Sex { get; init; }
        public required Country Country { get; init; }
        public required Data Data { get; init; }
    }
}
