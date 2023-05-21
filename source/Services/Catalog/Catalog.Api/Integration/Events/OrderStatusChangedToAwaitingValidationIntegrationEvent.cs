namespace Catalog.Api.Integration.Events
{
    public record OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
    {
        public OrderStatusChangedToAwaitingValidationIntegrationEvent(int orderId, IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }

        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }
    }
}