namespace Tenisu.Application.DTOs
{
    public record DataDTO
    {
        public int Rank { get; init; }
        public int Points { get; init; }
        public int Weight { get; init; }
        public int Height { get; init; }
        public int Age { get; init; }
        public List<int> Last { get; init; }

        public DataDTO(int rank, int points, int weight, int height, int age, List<int> last)
        {
            Rank = rank;
            Points = points;
            Weight = weight;
            Height = height;
            Age = age;
            Last = last;
        }
    }
}
