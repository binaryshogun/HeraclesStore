namespace Ordering.Api.Application.Events.Domain
{
    public class OrderCancelledDomainEventHandler
        : INotificationHandler<OrderCancelledDomainEvent>
    {
        private readonly ILogger<OrderCancelledDomainEventHandler> logger;

        public OrderCancelledDomainEventHandler(ILogger<OrderCancelledDomainEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(OrderCancelledDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.Order.Id, OrderStatus.Cancelled.Name, OrderStatus.Cancelled.Id);

            // TODO: Integration events support

            return Task.CompletedTask;
        }
    }
}