using System.Collections.Immutable;

namespace Tenisu.Domain.Entities
{
    public sealed record Data
    {
        public int Rank { get; private set; }
        public int Points { get; private set; }
        public int Weight { get; private set; }
        public int Height { get; private set; }
        public int Age { get; private set; }
        public ImmutableArray<int> Last { get;}

        private Data()
        {            
        }

        public Data(int rank, int points, int weight, int height, int age, IEnumerable<int> last)
        {
            Rank = rank;
            Points = points;
            Weight = weight;
            Height = height;
            Age = age;
            Last = last.ToImmutableArray();
        }

    }
}
