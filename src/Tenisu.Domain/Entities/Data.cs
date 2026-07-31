using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Tenisu.Domain.Entities
{
    public sealed record Data
    {
        public int Rank { get; private set; }
        public int Points { get; private set; }
        public int Weight { get; private set; }
        public int Height { get; private set; }
        public int Age { get; private set; }

        private readonly List<int> _last;
        public IReadOnlyCollection<int> Last => _last;

        private Data()
        {
            _last = null!;
        }

        [JsonConstructor]
        public Data(int rank, int points, int weight, int height, int age, IReadOnlyCollection<int> last)
        {
            Rank = rank;
            Points = points;
            Weight = weight;
            Height = height;
            Age = age;
            _last = last.ToList();
        }

    }
}
