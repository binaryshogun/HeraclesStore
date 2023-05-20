namespace Ordering.Api.Application.Events.Integration.Handlers
{
    public class OrderPaymentSucceededIntegrationEventHandler :
        IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
    {
        private readonly IMediator mediator;
        private readonly ILogger<OrderPaymentSucceededIntegrationEventHandler> logger;

        public OrderPaymentSucceededIntegrationEventHandler(
            IMediator mediator,
            ILogger<OrderPaymentSucceededIntegrationEventHandler> logger)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderPaymentSucceededIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                var command = new SetPaidOrderStatusCommand(@event.OrderId);

                logger.LogInformation("[Ordering] ---> Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})", command.GetGenericTypeName(), nameof(command.OrderId), command.OrderId, command);

                await mediator.Send(command);
            }
        }
    }
}