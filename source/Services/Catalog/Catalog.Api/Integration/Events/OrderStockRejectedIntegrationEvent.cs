namespace Catalog.Api.Integration.Events
{
    public record OrderStockRejectedIntegrationEvent : IntegrationEvent
    {
        public OrderStockRejectedIntegrationEvent(int orderId, List<ConfirmedOrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }

        public int OrderId { get; }
        public List<ConfirmedOrderStockItem> OrderStockItems { get; }
    }
}