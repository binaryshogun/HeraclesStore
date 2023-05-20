namespace Ordering.Api.Application.Events.Domain
{
    public class OrderItemsStockConfirmedDomainEventHandler
        : INotificationHandler<OrderItemsStockConfirmedDomainEvent>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IBuyerRepository buyerRepository;
        private readonly IOrderingIntegrationEventService orderingIntegrationEventService;
        private readonly ILogger<OrderItemsStockConfirmedDomainEventHandler> logger;

        public OrderItemsStockConfirmedDomainEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<OrderItemsStockConfirmedDomainEventHandler> logger)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            this.orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderItemsStockConfirmedDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.OrderId, OrderStatus.StockConfirmed.Name, OrderStatus.StockConfirmed.Id);

            var order = await orderRepository.GetAsync(notification.OrderId);
            var buyer = await buyerRepository.GetByIdAsync(order?.BuyerId ?? 0);

            if (order is not null && buyer is not null)
            {
                var integrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
                await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
            }
        }
    }
}