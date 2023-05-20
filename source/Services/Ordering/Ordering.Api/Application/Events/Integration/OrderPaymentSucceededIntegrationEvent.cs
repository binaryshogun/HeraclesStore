namespace Ordering.Api.Application.Events.Integration
{
    public record OrderPaymentSucceededIntegrationEvent : IntegrationEvent
    {
        public OrderPaymentSucceededIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; }
    }
}