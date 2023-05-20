namespace Ordering.Api.Application.Events.Domain
{
    public class OrderAwaitingValidationDomainEventHandler
        : INotificationHandler<OrderAwaitingValidationDomainEvent>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IBuyerRepository buyerRepository;
        private readonly IOrderingIntegrationEventService orderingIntegrationEventService;
        private readonly ILogger<OrderAwaitingValidationDomainEventHandler> logger;

        public OrderAwaitingValidationDomainEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<OrderAwaitingValidationDomainEventHandler> logger)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            this.orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderAwaitingValidationDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.OrderId, OrderStatus.AwaitingValidation.Name, OrderStatus.AwaitingValidation.Id);

            var order = await orderRepository.GetAsync(notification.OrderId);
            var buyer = await buyerRepository.GetByIdAsync(order?.BuyerId ?? 0);

            if (order is not null && buyer is not null)
            {
                var orderStockList = notification.OrderItems.Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units));
                var integrationEvent = new OrderStatusChangedToAwaitingValidationIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name, orderStockList);
            }
        }
    }
}