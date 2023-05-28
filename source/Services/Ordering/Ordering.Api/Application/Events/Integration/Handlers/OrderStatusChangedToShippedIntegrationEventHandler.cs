namespace Ordering.Api.Application.Events.Integration.Handlers
{
    public class OrderStatusChangedToShippedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToShippedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> hubContext;
        private readonly ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger;

        public OrderStatusChangedToShippedIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger)
        {
            this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Handle(OrderStatusChangedToShippedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                logger.LogInformation("[Ordering] ---> Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                await hubContext.Clients
                    .Group(@event.BuyerName)
                    .SendAsync("UpdatedOrderState", new { OrderId = @event.OrderId, Status = @event.OrderStatus });
            }
        }
    }
}