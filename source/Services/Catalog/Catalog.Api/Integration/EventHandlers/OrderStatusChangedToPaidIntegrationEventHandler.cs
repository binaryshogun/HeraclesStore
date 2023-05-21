namespace Catalog.Api.Integration.EventHandlers
{
    public class OrderStatusChangedToPaidIntegrationEventHandler
        : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        private readonly CatalogContext catalogContext;
        private readonly ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger;

        public OrderStatusChangedToPaidIntegrationEventHandler(
            CatalogContext catalogContext,
            ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
        {
            this.catalogContext = catalogContext;
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                logger.LogInformation("[Catalog] ---> Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                foreach (var orderStockItem in @event.OrderStockItems)
                {
                    var catalogItem = catalogContext.CatalogItems.Find(orderStockItem.ProductId);

                    catalogItem?.RemoveStock(orderStockItem.Units);
                }

                await catalogContext.SaveChangesAsync();
            }
        }
    }
}
