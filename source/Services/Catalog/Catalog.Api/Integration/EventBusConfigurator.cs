namespace Catalog.Api.Integration
{
    public static class EventBusConfigurator
    {
        public static void ConfigureEventBus(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            SubscribeOnEvents(scope.ServiceProvider.GetRequiredService<IEventBus>());
        }

        private static void SubscribeOnEvents(IEventBus eventBus)
        {
            eventBus.Subscribe<OrderStatusChangedToAwaitingValidationIntegrationEvent, OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
            eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
        }
    }
}