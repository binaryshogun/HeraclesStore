namespace EventBus.Abstractions
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string>? OnEventRemoved;

        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void AddDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        Type? GetEventTypeByName(string eventName);
        string GetEventKey<T>();
        void Clear();

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo?> GetHandlersForEvent<T>() where T : IntegrationEvent;

        bool HasSubscriptionsForEvent(string eventName);
        IEnumerable<SubscriptionInfo?> GetHandlersForEvent(string eventName);
    }
}