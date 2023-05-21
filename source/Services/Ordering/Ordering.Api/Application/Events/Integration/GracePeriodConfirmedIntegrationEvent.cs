namespace Ordering.Api.Application.Events.Integration
{
    public record GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public GracePeriodConfirmedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; }
    }
}