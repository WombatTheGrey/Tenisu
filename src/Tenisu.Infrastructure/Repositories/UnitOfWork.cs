using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tenisu.Domain.Exceptions;
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

        public async Task SaveEntitiesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) 
            {
                if(ex.InnerException is SqlException sqlException && sqlException.Number is 2601 or 2627)//error numbers for unique key or index violation
                {
                    throw new EntityAlreadyExistsException("An entity with the same unique Key or Index already exists", ex);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
