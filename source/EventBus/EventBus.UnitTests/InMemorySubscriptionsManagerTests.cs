namespace EventBus.UnitTests
{
    public class InMemorySubscriptionsManagerTests
    {
        [Fact]
        public void Create_ManagerShouldBeEmpty()
        {
            // Given
            var manager = new InMemoryEventBusSubscriptionsManager();

            // Then
            Assert.True(manager.IsEmpty);
        }

        [Fact]
        public void AddSubscription_ManagerShouldContainSubscriptionForEvent()
        {
            // Given
            var manager = new InMemoryEventBusSubscriptionsManager();

            // When
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

            // Then
            Assert.True(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        [Fact]
        public void RemoveSubscription_ManagerShouldNotContainSubscriptionForEvent()
        {
            // Given
            var manager = new InMemoryEventBusSubscriptionsManager();

            // When
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

            // Then
            Assert.False(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        [Fact]
        public void RemoveEvent_RaiseEvent_OnEventRemoved_EventShouldBeRaised()
        {
            // Given
            bool raised = false;
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.OnEventRemoved += (o, e) => raised = true;

            // When
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

            // Then
            Assert.True(raised);
        }

        [Fact]
        public void GetHandlersForEvent_MultipleHandlers_ShouldReturnAllHandlers()
        {
            // Given
            var manager = new InMemoryEventBusSubscriptionsManager();

            // When
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.AddSubscription<TestIntegrationEvent, OtherTestIntegrationEventHandler>();

            // Then
            var handlers = manager.GetHandlersForEvent<TestIntegrationEvent>();
            Assert.Equal(2, handlers.Count());
        }
    }
}