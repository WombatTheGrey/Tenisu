using Tenisu.Domain.Exceptions;

namespace Tenisu.Domain.Entities
{
    //Owned type
    public sealed record Data
    {
        public int Rank { get; init; }
        public int Points { get; init; }
        public int Weight { get; init; }
        public int Height { get; init; }
        public int Age { get; init; }
        public IReadOnlyCollection<int> Last { get; init; }

        private Data()
        {
            Last = null!;
        }

        public Data(int rank, int points, int weight, int height, int age, IReadOnlyCollection<int> last)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rank);
            ArgumentOutOfRangeException.ThrowIfNegative(points);
            ArgumentOutOfRangeException.ThrowIfNegative(weight);
            ArgumentOutOfRangeException.ThrowIfNegative(height);
            ArgumentOutOfRangeException.ThrowIfNegative(age);
            ArgumentNullException.ThrowIfNull(last);

            Rank = rank;
            Points = points;
            Weight = weight;
            Height = height;
            Age = age;

            if(last.Count > 5)
            {
                throw new DomainException("Last should contain at most 5 matches");
            }

            Last = last;
        }

    }
}
