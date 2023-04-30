namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// Repository contract.
    /// </summary>
    /// <typeparam name="T">Entity managed by repository.</typeparam>
    public interface IRepository<T> where T : Entity
    {
        /// <summary>
        /// Unit of work for repository.
        /// </summary>
        /// <value><see cref="IUnitOfWork" /></value>
        IUnitOfWork UnitOfWork { get; }
    }
}