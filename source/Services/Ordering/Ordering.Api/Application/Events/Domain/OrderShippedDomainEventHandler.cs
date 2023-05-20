namespace Ordering.Api.Application.Events.Domain
{
    public class OrderShippedDomainEventHandler
        : INotificationHandler<OrderShippedDomainEvent>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IBuyerRepository buyerRepository;
        private readonly IOrderingIntegrationEventService orderingIntegrationEventService;
        private readonly ILogger<OrderShippedDomainEventHandler> logger;

        public OrderShippedDomainEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<OrderShippedDomainEventHandler> logger)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            this.orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderShippedDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.Order.Id, OrderStatus.Shipped.Name, OrderStatus.Shipped.Id);

            var order = await orderRepository.GetAsync(notification.Order.Id);
            var buyer = await buyerRepository.GetByIdAsync(order?.BuyerId ?? 0);

            if (order is not null && buyer is not null)
            {
                var orderStatusChangedToShippedIntegrationEvent = new OrderStatusChangedToShippedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
                await orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToShippedIntegrationEvent);
            }
        }
    }
}