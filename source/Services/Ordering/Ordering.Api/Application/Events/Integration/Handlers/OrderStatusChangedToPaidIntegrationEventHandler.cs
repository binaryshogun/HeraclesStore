namespace Ordering.Api.Application.Events.Integration.Handlers
{
    public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> hubContext;
        private readonly ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger;

        public OrderStatusChangedToPaidIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
        {
            this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
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