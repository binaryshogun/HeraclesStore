namespace Ordering.Api.Application.Events.Domain
{
    public class OrderItemsStockConfirmedDomainEventHandler
        : INotificationHandler<OrderItemsStockConfirmedDomainEvent>
    {
        private readonly ILogger<OrderItemsStockConfirmedDomainEventHandler> logger;

        public OrderItemsStockConfirmedDomainEventHandler(ILogger<OrderItemsStockConfirmedDomainEventHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(OrderItemsStockConfirmedDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.OrderId, OrderStatus.StockConfirmed.Name, OrderStatus.StockConfirmed.Id);

            // TODO: Integration events support

            return Task.CompletedTask;
        }
    }
}