namespace Catalog.Api.Integration.Services
{
    public interface ICatalogIntegrationEventService
    {
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent @event);
        Task PublishThroughEventBusAsync(IntegrationEvent @event);
    }
}