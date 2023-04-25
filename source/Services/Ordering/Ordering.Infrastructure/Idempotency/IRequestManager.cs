namespace Ordering.Infrastructure.Idempotency
{
    /// <summary>
    /// Request manager contract.
    /// </summary>
    public interface IRequestManager
    {
        /// <summary>
        /// Specifies whether the <see cref="ClientRequest" /> with given <paramref name="id" /> exists.
        /// </summary>
        /// <param name="id">Client request identifier.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation and contains
        /// <see langword="true" /> if request exists, otherwise - <see langword="false" />.</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Creates new request for command of type <typeparamref name="T" /> 
        /// with given unique <paramref name="id" />.
        /// </summary>
        /// <param name="id">Request identifier.</param>
        /// <typeparam name="T">Command type.</typeparam>
        /// <returns><see cref="Task" /> that represents asynchronous operation.</returns>
        Task CreateRequestForCommandAsync<T>(Guid id);
    }
}