namespace Tenisu.Domain.Entities
{
    public record Data//flatten in EF
    {
        public required int Rank { get; init; }
        public required int Points { get; init; }
        public required int Weight { get; init; }
        public required int Height { get; init; }
        public required int Age { get; init; }
        public required int[] Last { get; init; }

    }
}
