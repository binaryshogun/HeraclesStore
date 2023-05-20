namespace Ordering.Api.Application.Events.Integration
{
    public record OrderStartedIntegrationEvent : IntegrationEvent
    {
        public OrderStartedIntegrationEvent(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}