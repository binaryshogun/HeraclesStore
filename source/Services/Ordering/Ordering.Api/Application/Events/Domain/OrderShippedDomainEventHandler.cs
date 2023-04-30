namespace Ordering.Api.Application.Events.Domain
{
    public class OrderShippedDomainEventHandler
        : INotificationHandler<OrderShippedDomainEvent>
    {
        private readonly ILogger<OrderShippedDomainEventHandler> logger;

        public OrderShippedDomainEventHandler(ILogger<OrderShippedDomainEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(OrderShippedDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.Order.Id, OrderStatus.Shipped.Name, OrderStatus.Shipped.Id);

            // TODO: Integration events support

            return Task.CompletedTask;
        }
    }
}