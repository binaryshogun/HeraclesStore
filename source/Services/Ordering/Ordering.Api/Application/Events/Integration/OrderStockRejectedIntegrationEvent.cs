namespace Ordering.Api.Application.Events.Integration
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