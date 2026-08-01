using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Tenisu.Domain.Entities;
using Tenisu.Infrastructure.Context;

namespace Tenisu.Infrastructure.Initialization
{
    internal static class TenisuSeeder
    {
        internal static async Task SeedAsync(TenisuDbContext dbContext, CancellationToken cancellationToken)
        {
            var jsonPlayers = await TenisuJsonDeserializer.DeserializePlayersAsync(cancellationToken)
                ?? throw new InvalidOperationException("Failed to deserialize players from JSON.");

            await SeedCountriesAsync(dbContext, jsonPlayers, cancellationToken);
            await SeedPlayersAsync(dbContext, jsonPlayers, cancellationToken);
        }

        private static async Task SeedCountriesAsync(TenisuDbContext dbContext, IEnumerable<Player> jsonPlayers, CancellationToken cancellationToken)
        {
            if (await dbContext.Countries.AnyAsync(cancellationToken))
            {
                return;
            }

            var countries = jsonPlayers.Select(p => p.Country).Distinct().ToList();

            dbContext.AddRange(countries);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task SeedPlayersAsync(TenisuDbContext dbContext, IEnumerable<Player> jsonPlayers, CancellationToken cancellationToken)
        {
            if (await dbContext.Players.AnyAsync(cancellationToken: cancellationToken))
            {
                return;
            }

            var countries = await dbContext.Countries.ToDictionaryAsync(c=> c.Code, cancellationToken);

            var players = new List<Player>();

            foreach (var jsonPlayer in jsonPlayers)
            {
                if(!countries.TryGetValue(jsonPlayer.Country.Code, out var country))
                {
                    throw new InvalidOperationException($"The following country is not available in the Database : {jsonPlayer.Country.Code}");
                }

                var player = new Player(jsonPlayer.Id,
                    jsonPlayer.FirstName,
                    jsonPlayer.LastName,
                    jsonPlayer.Sex,
                    country,
                    jsonPlayer.Picture,
                    jsonPlayer.Data);

                players.Add(player);
            }

            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Players ON", cancellationToken);
            
            dbContext.Players.AddRange(players);
            await dbContext.SaveChangesAsync(cancellationToken);

            await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Players OFF", cancellationToken);
        }
    }
}
