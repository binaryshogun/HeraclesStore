namespace Ordering.Domain.Events
{
    public class OrderItemsStockConfirmedDomainEvent : INotification
    {
        public int OrderId { get; }

        public OrderItemsStockConfirmedDomainEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}