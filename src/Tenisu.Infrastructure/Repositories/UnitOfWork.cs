using Tenisu.Domain.Interfaces;
using Tenisu.Infrastructure.Context;

namespace Tenisu.Infrastructure.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly TenisuDbContext _dbContext;
        public UnitOfWork(TenisuDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task SaveEntitiesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
