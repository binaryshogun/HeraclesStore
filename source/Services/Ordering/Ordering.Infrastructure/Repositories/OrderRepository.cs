namespace Ordering.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of repository for <see cref="Order" /> entity 
    /// that uses EntityFrameworkCore for operations.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderingContext context;

        /// <summary>
        /// Creates new instance of <see cref="OrderRepository" /> with given <paramref name="context" />.
        /// </summary>
        /// <param name="context">Current <see cref="OrderingContext" />.</param>
        /// <exception cref="ArgumentNullException" />
        public OrderRepository(OrderingContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => context;

        public Order? Get(int orderId)
        {
            return context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderStatus)
                .Where(o => o.Id == orderId)
                .FirstOrDefault();
        }

        public async Task<Order?> GetAsync(int orderId)
        {
            return await context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderStatus)
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public Order Add(Order order)
        {
            if (order.IsTransient())
            {
                if (order.OrderStatus is not null)
                {
                    context.OrderStatus.Attach(order.OrderStatus);
                }
                return context.Orders.Add(order).Entity;
            }

            return order;
        }

        public Order Update(Order order)
        {
            return context.Orders.Update(order).Entity;
        }
    }
}