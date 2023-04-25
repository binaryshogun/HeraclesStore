namespace Ordering.Api.Application.Events.Domain
{
    public class OrderPaidDomainEventHandler
        : INotificationHandler<OrderPaidDomainEvent>
    {
        private readonly ILogger<OrderPaidDomainEventHandler> logger;

        public OrderPaidDomainEventHandler(ILogger<OrderPaidDomainEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(OrderPaidDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.OrderId, OrderStatus.Paid.Name, OrderStatus.Paid.Id);

            // TODO: Integration events support

            return Task.CompletedTask;
        }
    }
}