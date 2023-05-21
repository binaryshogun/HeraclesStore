namespace Catalog.Api.Integration.Events
{
    public record OrderStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public OrderStockConfirmedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; }
    }
}