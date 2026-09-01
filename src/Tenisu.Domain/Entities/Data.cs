using Tenisu.Domain.Exceptions;

namespace Tenisu.Domain.Entities
{
    //Owned type
    public sealed record Data
    {
        /// <summary>Rank of the player. n°1 is the best ranked player </summary>
        public int Rank { get; init; }
        /// <summary> Total number of points </summary>
        public int Points { get; init; }
        /// <summary> Weight in grams </summary>
        public int Weight { get; init; }
        /// <summary> Height in cm </summary>
        public int Height { get; init; }
        /// <summary> Age </summary>
        public int Age { get; init; }
        /// <summary> Results of the last 5 matches </summary>
        public IReadOnlyCollection<int> Last { get; init; }//represented as int for convenance. But could be a more appropriate datatype because the values are only 1 and 0.

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
            if(last.Any(l => l != 0 && l != 1))
            {
                throw new DomainException("Last matches can only be victories or defeats. So Last must contain only 1 and 0");
            }

            Last = last;
        }

    }
}
