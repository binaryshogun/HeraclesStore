namespace Ordering.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken token = default);

        bool SaveEntities();
        Task<bool> SaveEntitiesAsync(CancellationToken token = default);
    }
}