namespace Ordering.Api.Application.Events.Domain
{
    public class OrderCancelledDomainEventHandler
        : INotificationHandler<OrderCancelledDomainEvent>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IBuyerRepository buyerRepository;
        private readonly IOrderingIntegrationEventService orderingIntegrationEventService;
        private readonly ILogger<OrderCancelledDomainEventHandler> logger;

        public OrderCancelledDomainEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<OrderCancelledDomainEventHandler> logger)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            this.orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderCancelledDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.Order.Id, OrderStatus.Cancelled.Name, OrderStatus.Cancelled.Id);

            var order = await orderRepository.GetAsync(notification.Order.Id);

            var buyer = await buyerRepository.GetByIdAsync(order?.BuyerId ?? 0);

            if (order is not null && buyer is not null)
            {
                var integrationEvent = new OrderStatusChangedToCancelledIntegrationEvent(order.Id, OrderStatus.Cancelled.Name, buyer.Name);
                await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
            }
        }
    }
}