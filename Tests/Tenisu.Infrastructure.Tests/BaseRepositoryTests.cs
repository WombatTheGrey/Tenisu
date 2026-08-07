using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tenisu.Infrastructure.Context;

namespace Tenisu.Infrastructure.Tests
{
    [Parallelizable(ParallelScope.Fixtures)]
    public abstract class BaseRepositoryTests
    {
        private SqliteConnection _sqliteConnection;
        protected TenisuDbContext DbContext;
        protected CancellationToken CancellationToken = TestContext.CurrentContext.CancellationToken;

        [SetUp]
        public async Task Setup()
        {
            _sqliteConnection = new SqliteConnection("Filename=:memory:");
            await _sqliteConnection.OpenAsync(CancellationToken);

            var options = new DbContextOptionsBuilder<TenisuDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;

            DbContext = new TenisuDbContext(options);

            await DbContext.Database.EnsureCreatedAsync(CancellationToken);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _sqliteConnection.CloseAsync();
            await _sqliteConnection.DisposeAsync();
            await DbContext.DisposeAsync();
        }

    }
}
