namespace Ordering.Api.Application.Events.Integration.Handlers
{
    public class OrderPaymentFailedIntegrationEventHandler :
        IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly IMediator mediator;
        private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> logger;

        public OrderPaymentFailedIntegrationEventHandler(
            IMediator mediator,
            ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                logger.LogInformation("[Ordering] ---> Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                var command = new CancelOrderCommand(@event.OrderId);

                logger.LogInformation("[Ordering] ---> Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})", command.GetGenericTypeName(), nameof(command.OrderId), command.OrderId, command);

                await mediator.Send(command);
            }
        }
    }
}
