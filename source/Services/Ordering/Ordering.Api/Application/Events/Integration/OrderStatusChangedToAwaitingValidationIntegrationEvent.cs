namespace Ordering.Api.Application.Events.Integration
{
    public record OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
    {
        public OrderStatusChangedToAwaitingValidationIntegrationEvent(int orderId, string orderStatus, string buyerName, IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
            OrderStockItems = orderStockItems;
        }

        public int OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }
    }
}