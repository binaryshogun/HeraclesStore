namespace Ordering.Domain.Models.BuyerAggregate
{
    /// <summary>
    /// Repository contract for <see cref="Buyer" /> entity.
    /// </summary>
    public interface IBuyerRepository : IRepository<Buyer>
    {
        /// <summary>
        /// Gets buyer from data source by <paramref name="id" />.
        /// </summary>
        /// <param name="id">Buyer identifier.</param>
        /// <returns><see cref="Buyer" /> instance if exists; otherwise - <see langword="null" />.</returns>
        Buyer? GetById(int id);

        /// <summary>
        /// Asynchronously gets buyer from data source by <paramref name="id" />.
        /// </summary>
        /// <param name="id">Buyer identifier.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation and contains <see cref="Buyer" /> 
        /// instance if exists, otherwise - <see langword="null" />.</returns>
        Task<Buyer?> GetByIdAsync(int id);

        /// <summary>
        /// Gets buyer from data source by <paramref name="identityId" />.
        /// </summary>
        /// <param name="identityId">Buyer identity identifier.</param>
        /// <returns><see cref="Buyer" /> instance if exists; otherwise - <see langword="null" />.</returns>
        Buyer? GetByIdentity(Guid identityId);

        /// <summary>
        /// Asynchronously gets buyer from data source by <paramref name="identityId" />.
        /// </summary>
        /// <param name="identityId">Buyer identity identifier.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation and contains <see cref="Buyer" /> 
        /// instance if exists, otherwise - <see langword="null" />.</returns>
        Task<Buyer?> GetByIdentityAsync(Guid identityId);

        /// <summary>
        /// Adds buyer to data source.
        /// </summary>
        /// <param name="buyer"><see cref="Buyer" /> instance that should be added to data source.</param>
        /// <returns><see cref="Buyer" /> instance that was added to data source.</returns>
        Buyer Add(Buyer buyer);

        /// <summary>
        /// Asynchronously adds buyer to data source.
        /// </summary>
        /// <param name="buyer"><see cref="Buyer" /> instance that should be added to data source.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation and contains 
        /// <see cref="Buyer" /> instance that was added to data source.</returns>
        Task<Buyer> AddAsync(Buyer buyer);

        /// <summary>
        /// Updates buyer entity in data source.
        /// </summary>
        /// <param name="buyer"><see cref="Buyer" /> instance that should be updated.</param>
        /// <returns><see langword="true" /> if <paramref name="buyer" /> was successfully 
        /// updated; otherwise - <see langword="false" />.</returns>
        bool Update(Buyer buyer);

        /// <summary>
        /// Asynchronously updates buyer entity in data source.
        /// </summary>
        /// <param name="buyer"><see cref="Buyer" /> instance that should be updated.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation and contains 
        /// <see langword="true" /> if <paramref name="buyer" /> was successfully 
        /// updated, otherwise - <see langword="false" />.</returns>
        Task<bool> UpdateAsync(Buyer buyer);
    }
}