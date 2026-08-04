using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Tenisu.Infrastructure.Context;

namespace Tenisu.Infrastructure.Tests
{
    public abstract class BaseRepositoryTests
    {
        private SqliteConnection _sqliteConnection;
        protected TenisuDbContext DbContext;

        [SetUp]
        public async Task Setup()
        {
            var cancellationToken = TestContext.CurrentContext.CancellationToken;
            _sqliteConnection = new SqliteConnection("Filename=:memory");
            await _sqliteConnection.OpenAsync(cancellationToken);

            var options = new DbContextOptionsBuilder<TenisuDbContext>()
                .UseSqlite()
                .Options;

            DbContext = new TenisuDbContext(options);

            await DbContext.Database.MigrateAsync(cancellationToken);
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
