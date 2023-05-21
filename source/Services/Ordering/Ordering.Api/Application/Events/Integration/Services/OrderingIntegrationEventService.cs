namespace Ordering.Api.Application.Events.Integration.Services
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly IEventBus eventBus;
        private readonly OrderingContext orderingContext;
        private readonly IIntegrationEventLogService eventLogService;
        private readonly ILogger<OrderingIntegrationEventService> logger;

        public OrderingIntegrationEventService(IEventBus eventBus,
            OrderingContext orderingContext,
            IntegrationEventLogContext eventLogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<OrderingIntegrationEventService> logger)
        {
            this.orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this.eventLogService = integrationEventLogServiceFactory is not null ? integrationEventLogServiceFactory(this.orderingContext.Database.GetDbConnection()) : throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (IntegrationEventLogEntry logEvent in pendingLogEvents)
            {
                logger.LogInformation("[Ordering] ---> Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", logEvent.EventId, logEvent.IntegrationEvent);

                try
                {
                    await eventLogService.MarkEventAsInProgressAsync(logEvent.EventId);
                    eventBus.PublishEvent(logEvent.IntegrationEvent!);
                    await eventLogService.MarkEventAsPublishedAsync(logEvent.EventId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "[Ordering] ---> ERROR publishing integration event: {IntegrationEventId}", logEvent.EventId);

                    await eventLogService.MarkEventAsFailedAsync(logEvent.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent @event)
        {
            logger.LogInformation("[Ordering] ---> Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", @event.Id, @event);

            if (orderingContext.CurrentTransaction is not null)
            {
                await eventLogService.SaveEventAsync(@event, orderingContext.CurrentTransaction);
            }

            logger.LogWarning("[Ordering] ---> Integration event wasn't enqueued because transaction is empty");
        }
    }
}