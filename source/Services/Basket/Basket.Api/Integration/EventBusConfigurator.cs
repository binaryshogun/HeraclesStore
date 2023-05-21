namespace Basket.Api.Integration
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
            eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
            eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        }
    }
}