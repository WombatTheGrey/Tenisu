namespace Tenisu.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveEntitiesAsync(CancellationToken cancellationToken);
    }
}
