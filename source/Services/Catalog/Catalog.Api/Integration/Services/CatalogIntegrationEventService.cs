namespace Catalog.Api.Integration.Services
{
    public class CatalogIntegrationEventService : ICatalogIntegrationEventService, IDisposable
    {
        private readonly IEventBus eventBus;
        private readonly CatalogContext catalogContext;
        private readonly IIntegrationEventLogService eventLogService;
        private readonly ILogger<CatalogIntegrationEventService> logger;
        private volatile bool disposedValue;

        public CatalogIntegrationEventService(
            CatalogContext catalogContext,
            IEventBus eventBus,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<CatalogIntegrationEventService> logger)
        {
            this.catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this.eventLogService = integrationEventLogServiceFactory is not null ? integrationEventLogServiceFactory(this.catalogContext.Database.GetDbConnection()) : throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                logger.LogInformation("[Catalog] ---> Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})", evt.Id, evt);

                await eventLogService.MarkEventAsInProgressAsync(evt.Id);
                eventBus.PublishEvent(evt);
                await eventLogService.MarkEventAsPublishedAsync(evt.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Catalog] ---> ERROR Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", evt.Id, evt);
                await eventLogService.MarkEventAsFailedAsync(evt.Id);
            }
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
        {
            logger.LogInformation("[Catalog] ---> CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", evt.Id);

            await ResilientTransaction.New(catalogContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                await catalogContext.SaveChangesAsync();
                await eventLogService.SaveEventAsync(evt, catalogContext.Database.CurrentTransaction!);
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    (eventLogService as IDisposable)?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}