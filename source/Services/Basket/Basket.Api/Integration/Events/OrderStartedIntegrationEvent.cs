namespace Basket.Api.IntegrationEvents.Events
{
    public record OrderStartedIntegrationEvent : IntegrationEvent
    {
        public OrderStartedIntegrationEvent(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; init; }
    }
}