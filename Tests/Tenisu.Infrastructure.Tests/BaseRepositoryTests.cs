using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tenisu.Domain.Entities;
using Tenisu.Domain.Interfaces;
using Tenisu.Infrastructure.Context;
using Tenisu.Infrastructure.Repositories;

namespace Tenisu.Infrastructure.Tests
{
    [Parallelizable(ParallelScope.Fixtures)]
    public abstract class BaseRepositoryTests
    {
        private SqliteConnection _sqliteConnection;
        protected TenisuDbContext DbContext;
        protected IUnitOfWork UnitOfWork;
        protected CancellationToken CancellationToken = TestContext.CurrentContext.CancellationToken;

        protected const string CountryCode = "FRA";
        protected static readonly Country Country = new Country(new Uri("http://localhost"), CountryCode);

        [SetUp]
        public async Task Setup()
        {
            _sqliteConnection = new SqliteConnection("Filename=:memory:");
            await _sqliteConnection.OpenAsync(CancellationToken);

            var options = new DbContextOptionsBuilder<TenisuDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;

            DbContext = new TenisuDbContext(options);
            UnitOfWork = new UnitOfWork(DbContext);

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
