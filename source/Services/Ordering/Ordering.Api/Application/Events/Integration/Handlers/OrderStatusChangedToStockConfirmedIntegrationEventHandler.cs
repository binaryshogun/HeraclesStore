namespace Ordering.Api.Application.Events.Integration.Handlers
{
    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> hubContext;
        private readonly ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(
            IHubContext<NotificationsHub> hubContext,
            ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger)
        {
            this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
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