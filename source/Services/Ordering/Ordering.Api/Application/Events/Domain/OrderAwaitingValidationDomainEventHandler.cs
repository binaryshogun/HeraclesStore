namespace Ordering.Api.Application.Events.Domain
{
    public class OrderAwaitingValidationDomainEventHandler
        : INotificationHandler<OrderAwaitingValidationDomainEvent>
    {
        private readonly ILogger<OrderAwaitingValidationDomainEventHandler> logger;

        public OrderAwaitingValidationDomainEventHandler(ILogger<OrderAwaitingValidationDomainEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(OrderAwaitingValidationDomainEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("[Ordering] ---> Order #{Id} has been successfully updated to status {Status} ({StatusId})",
                notification.OrderId, OrderStatus.AwaitingValidation.Name, OrderStatus.AwaitingValidation.Id);

            // TODO: Integration events support

            return Task.CompletedTask;
        }
    }
}