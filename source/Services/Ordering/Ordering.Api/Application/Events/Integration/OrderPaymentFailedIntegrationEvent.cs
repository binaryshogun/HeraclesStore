namespace Ordering.Api.Application.Events.Integration
{
    public record OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public OrderPaymentFailedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; }
    }
}