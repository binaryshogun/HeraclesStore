namespace Ordering.Api.Infrastructure
{
    public static class EventBusConfiguratior
    {
        public static void ConfigureEventBus(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

            SubscribeOnEvents(eventBus);
        }

        private static void SubscribeOnEvents(IEventBus eventBus)
        {
            eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
            eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>>();
            eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>>();
            eventBus.Subscribe<OrderStockRejectedIntegrationEvent, IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>>();
            eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>>();
            eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>>();
        }
    }
}