namespace Catalog.Api.Integration.EventHandlers
{
    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler
        : IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
    {
        private readonly CatalogContext catalogContext;
        private readonly ICatalogIntegrationEventService catalogIntegrationEventService;
        private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(
            CatalogContext catalogContext,
            ICatalogIntegrationEventService catalogIntegrationEventService,
            ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger)
        {
            this.catalogContext = catalogContext;
            this.catalogIntegrationEventService = catalogIntegrationEventService;
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                logger.LogInformation("[Catalog] ---> Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

                foreach (var orderStockItem in @event.OrderStockItems)
                {
                    var catalogItem = catalogContext.CatalogItems.Find(orderStockItem.ProductId);
                    var hasStock = catalogItem?.AvailableInStock >= orderStockItem.Units;
                    var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem?.Id ?? 0, hasStock);

                    confirmedOrderStockItems.Add(confirmedOrderStockItem);
                }

                var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
                    ? new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems) as IntegrationEvent
                    : new OrderStockConfirmedIntegrationEvent(@event.OrderId) as IntegrationEvent;

                await catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(confirmedIntegrationEvent);
                await catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);
            }
        }
    }
}
