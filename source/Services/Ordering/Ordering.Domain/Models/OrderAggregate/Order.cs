namespace Ordering.Domain.Models.OrderAggregate
{
    /// <summary>
    /// Represents order entity.
    /// </summary>
    public class Order : Entity
    {
        private DateTime orderDate;
        private string? description;
        private int? buyerId;
        private int orderStatusId;
        private int? paymentMethodId;

        private readonly List<OrderItem> orderItems;

        protected Order()
        {
            orderItems = new List<OrderItem>();
        }

        /// <summary>
        /// Creates new instance of <see cref="Order" />.
        /// </summary>
        /// <param name="userId">User identity identifier.</param>
        /// <param name="userName">User identity name.</param>
        /// <param name="address">Shipment adress.</param>
        /// <param name="cardTypeId">Card type identifier.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <param name="cardSecurityNumber">Card security number.</param>
        /// <param name="cardHolderName">Card holder name.</param>
        /// <param name="cardExpiration">Card expiration date.</param>
        /// <param name="buyerId">Buyer identifier.</param>
        /// <param name="paymentMethodId">Payment method identifier.</param>
        /// <returns>New instance of <see cref="Order" />.</returns>
        public Order(Guid userId, string userName, Address address, int cardTypeId, string cardNumber, string cardSecurityNumber,
            string cardHolderName, DateTime cardExpiration, int? buyerId = null, int? paymentMethodId = null) : this()
        {
            BuyerId = buyerId;
            PaymentMethodId = paymentMethodId;
            this.orderStatusId = OrderStatus.Submitted.Id;
            this.orderDate = DateTime.UtcNow;

            Address = address;
            OrderStatus = OrderStatus.Submitted;

            AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
        }

        public static Order NewDraft()
        {
            return new Order();
        }

        public int? BuyerId
        {
            get => buyerId;
            set => buyerId = value;
        }
        public int? PaymentMethodId
        {
            get => paymentMethodId;
            set => paymentMethodId = value;
        }

        public Address Address { get; private set; } = default!;
        public OrderStatus OrderStatus { get; private set; } = default!;
        public IReadOnlyCollection<OrderItem> OrderItems => orderItems;

        public decimal Total => orderItems.Sum(o => o.Units * o.UnitPrice);

        /// <summary>
        /// Adds <see cref="OrderItem" /> with given <paramref name="productId" />, <paramref name="productId" />, 
        /// <paramref name="productId" />, <paramref name="productId" />, <paramref name="pictureUrl" /> and 
        /// <paramref name="units" /> to the <see cref="Order.OrderItems" />.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="productName">Product name.</param>
        /// <param name="unitPrice">Price per unit.</param>
        /// <param name="discount">Discount value.</param>
        /// <param name="pictureUrl">Url to product image.</param>
        /// <param name="units">Number of units.</param>
        public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string? pictureUrl = null, int units = 1)
        {
            var existingOrderForProduct = orderItems.Where(o => o.ProductId == productId)
                .SingleOrDefault();

            if (existingOrderForProduct is not null)
            {

                if (discount > existingOrderForProduct.Discount)
                {
                    existingOrderForProduct.Discount = discount;
                }

                existingOrderForProduct.AddUnits(units);
            }
            else
            {
                var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
                orderItems.Add(orderItem);
            }
        }

        /// <summary>
        /// Sets order status to awaiting validation.
        /// </summary>
        public void SetAwaitingValidationStatus()
        {
            if (orderStatusId == OrderStatus.Submitted.Id)
            {
                AddDomainEvent(new OrderAwaitingValidationDomainEvent(Id, OrderItems));
                orderStatusId = OrderStatus.AwaitingValidation.Id;
            }
        }

        /// <summary>
        /// Sets order status to confirmed items stock.
        /// </summary>
        public void SetStockConfirmedStatus()
        {
            if (orderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                AddDomainEvent(new OrderItemsStockConfirmedDomainEvent(Id));

                orderStatusId = OrderStatus.StockConfirmed.Id;
                description = "All the items were confirmed with available stock.";
            }
        }

        /// <summary>
        /// Sets order status to paid.
        /// </summary>
        public void SetPaidStatus()
        {
            if (orderStatusId == OrderStatus.StockConfirmed.Id)
            {
                AddDomainEvent(new OrderPaidDomainEvent(Id, OrderItems));

                orderStatusId = OrderStatus.Paid.Id;
                description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
            }
        }

        /// <summary>
        /// Sets order status to shipped.
        /// </summary>
        public void SetShippedStatus()
        {
            if (orderStatusId != OrderStatus.Paid.Id)
            {
                StatusChangeException(OrderStatus.Shipped);
            }

            AddDomainEvent(new OrderShippedDomainEvent(this));

            orderStatusId = OrderStatus.Shipped.Id;
            description = "The order was shipped.";
        }

        /// <summary>
        /// Set order status to cancelled.
        /// </summary>
        public void SetCancelledStatus()
        {
            if (orderStatusId == OrderStatus.Paid.Id || orderStatusId == OrderStatus.Shipped.Id)
            {
                StatusChangeException(OrderStatus.Cancelled);
            }

            AddDomainEvent(new OrderCancelledDomainEvent(this));

            orderStatusId = OrderStatus.Cancelled.Id;
            description = $"The order was cancelled.";
        }

        /// <summary>
        /// Canceling order if there is not enough items in stock.
        /// </summary>
        /// <param name="orderStockRejectedItems">Items that was rejected.</param>
        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
        {
            if (orderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                orderStatusId = OrderStatus.Cancelled.Id;

                var itemsStockRejectedProductNames = OrderItems
                    .Where(c => orderStockRejectedItems.Contains(c.ProductId))
                    .Select(c => c.ProductName);

                var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
                description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
            }
        }

        /// <summary>
        /// Throws exception with message about order status.
        /// </summary>
        /// <param name="orderStatusToChange">Order status that need to be changed to.</param>
        /// <exception cref="OrderingDomainException" />
        private void StatusChangeException(OrderStatus orderStatusToChange)
        {
            throw new OrderingDomainException($"Is not possible to change the order status from {OrderStatus.Name} to {orderStatusToChange.Name}.");
        }

        /// <summary>
        /// Sets order status to 'started'.
        /// </summary>
        /// <param name="userId">User identity identifier.</param>
        /// <param name="userName">User identity name.</param>
        /// <param name="cardTypeId">Card type identifier.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <param name="cardSecurityNumber">Card security number.</param>
        /// <param name="cardHolderName">Card holder name.</param>
        /// <param name="cardExpiration">Card expiration date.</param>
        private void AddOrderStartedDomainEvent(Guid userId, string userName, int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(
                this, userId, userName, cardTypeId, cardNumber,
                cardSecurityNumber, cardHolderName, cardExpiration);

            AddDomainEvent(orderStartedDomainEvent);
        }
    }
}