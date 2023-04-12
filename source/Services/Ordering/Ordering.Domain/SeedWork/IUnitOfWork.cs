namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// Contract for unit of work.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves all changes.
        /// </summary>
        /// <returns>Number of saved changes.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes. 
        /// </summary>
        /// <param name="token"><see cref="CancellationToken" /> responsible for canceling asynchronous operation.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation and contains number of saved changes in result.</returns>
        Task<int> SaveChangesAsync(CancellationToken token = default);

        /// <summary>
        /// Saves all entities.
        /// </summary>
        /// <returns><see langword="true" /> if any entity was saved; otherwise - <see langword="false" />.</returns>
        bool SaveEntities();

        /// <summary>
        /// Asynchronously saves all entities.
        /// </summary>
        /// <returns><see cref="Task" /> that represents asynchronous operation and contains <see langword="true" /> if any entity was saved, otherwise - <see langword="false" />.</returns>
        Task<bool> SaveEntitiesAsync(CancellationToken token = default);
    }
}