namespace EventBus.UnitTests
{
    public class OtherTestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; private set; }

        public OtherTestIntegrationEventHandler()
        {
            Handled = false;
        }

        public Task Handle(TestIntegrationEvent @event)
        {
            Handled = true;

            return Task.CompletedTask;
        }
    }
}