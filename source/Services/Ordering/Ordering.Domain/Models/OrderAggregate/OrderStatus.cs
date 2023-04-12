namespace Ordering.Domain.Models.OrderAggregate
{
    /// <summary>
    /// Enumeration representing order status collection.
    /// </summary>
    public class OrderStatus : Enumeration
    {
        public static OrderStatus Submitted = new OrderStatus(1, nameof(Submitted).ToLowerInvariant());
        public static OrderStatus AwaitingValidation = new OrderStatus(2, nameof(AwaitingValidation).ToLowerInvariant());
        public static OrderStatus StockConfirmed = new OrderStatus(3, nameof(StockConfirmed).ToLowerInvariant());
        public static OrderStatus Paid = new OrderStatus(4, nameof(Paid).ToLowerInvariant());
        public static OrderStatus Shipped = new OrderStatus(5, nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled).ToLowerInvariant());

        /// <summary>
        /// Creates new instance of Enumeration with given <paramref name="id" /> 
        /// and <paramref name="name" />.
        /// </summary>
        /// <param name="id">Enumeration item identifier.</param>
        /// <param name="name">Enumeration item name.</param>
        public OrderStatus(int id, string name)
            : base(id, name) { }

        /// <summary>
        /// Gets a list of all <see cref="OrderStatus" /> enumeration items.
        /// </summary>
        /// <returns><see cref="IEnumerable{OrderStatus}" /> of all <see cref="OrderStatus" /> enumeration items.</returns>
        public static IEnumerable<OrderStatus> List()
        {
            return new[]
            {
                Submitted,
                AwaitingValidation,
                StockConfirmed,
                Paid,
                Shipped,
                Cancelled
            };
        }

        /// <summary>
        /// Gets <see cref="OrderStatus" /> instance by <paramref name="name" />.
        /// </summary>
        /// <param name="name"><see cref="OrderStatus" /> name.</param>
        /// <returns><see cref="OrderStatus" /> instance found by <paramref name="name" />.</returns>
        public static OrderStatus GetByName(string name)
        {
            var state = List().SingleOrDefault(s =>
                string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        /// <summary>
        /// Gets <see cref="OrderStatus" /> instance by <paramref name="id" />.
        /// </summary>
        /// <param name="id"><see cref="OrderStatus" /> identifier.</param>
        /// <returns><see cref="OrderStatus" /> instance found by <paramref name="id" />.</returns>
        public static OrderStatus GetById(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}