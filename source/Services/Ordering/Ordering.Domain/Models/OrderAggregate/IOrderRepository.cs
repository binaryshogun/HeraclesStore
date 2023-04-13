namespace Ordering.Domain.Models.OrderAggregate
{
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Gets order from data source by <paramref name="orderId" />.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <returns><see cref="Order" /> instance if exists; otherwise - <see langword="null" />.</returns>
        Order? Get(int orderId);

        /// <summary>
        /// Asynchronously gets order from data source by <paramref name="orderId" />.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation and contains <see cref="Order" /> 
        /// instance if exists, otherwise - <see langword="null" />.</returns>
        Task<Order?> GetAsync(int orderId);

        /// <summary>
        /// Adds order to data source.
        /// </summary>
        /// <param name="order"><see cref="Order" /> instance that should be added to data source.</param>
        /// <returns><see cref="Order" /> instance that was added to data source.</returns>
        Order Add(Order order);

        /// <summary>
        /// Updates order entity in data source.
        /// </summary>
        /// <param name="order"><see cref="Order" /> instance that should be updated.</param>
        /// <returns><see langword="true" /> if <paramref name="order" /> was successfully 
        /// updated; otherwise - <see langword="false" />.</returns>
        bool Update(Order order);
    }
}