using System.Text.Json;
using System.Text.Json.Serialization;
using Tenisu.Domain.Entities;

namespace Tenisu.Infrastructure.Initialization
{
    internal static class TenisuJsonDeserializer
    {
        private static readonly Type CurrentType = typeof(TenisuJsonDeserializer);
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public async static Task<IEnumerable<Player>?> DeserializePlayersAsync(CancellationToken token)
        {
            using var stream = CurrentType.Assembly.GetManifestResourceStream($"{CurrentType.Namespace}.InitialData.json")
            ?? throw new InvalidOperationException("Seed file not found.");
            var jsonModel = await JsonSerializer.DeserializeAsync<PlayersJsonModel>(stream, JsonSerializerOptions, token);
            return jsonModel?.Players;
        }

        private record PlayersJsonModel
        {
            public IEnumerable<Player>? Players { get; set; }
        }
    }
}
