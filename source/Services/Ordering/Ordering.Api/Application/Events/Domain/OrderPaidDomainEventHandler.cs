namespace Ordering.Api.Application.Events.Domain
{
    public class OrderPaidDomainEventHandler
        : INotificationHandler<OrderPaidDomainEvent>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IBuyerRepository buyerRepository;
        private readonly IOrderingIntegrationEventService orderingIntegrationEventService;
        private readonly ILogger<OrderPaidDomainEventHandler> logger;

        public OrderPaidDomainEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<OrderPaidDomainEventHandler> logger)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            this.orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderPaidDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.OrderId, OrderStatus.Paid.Name, OrderStatus.Paid.Id);

            var order = await orderRepository.GetAsync(notification.OrderId);
            var buyer = await buyerRepository.GetByIdAsync(order?.BuyerId ?? 0);

            if (order is not null && buyer is not null)
            {
                var orderStockList = notification.OrderItems.Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units));

                var integrationEvent = new OrderStatusChangedToPaidIntegrationEvent(
                    notification.OrderId, order.OrderStatus.Name, buyer.Name, orderStockList);

                await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
            }
        }
    }
}