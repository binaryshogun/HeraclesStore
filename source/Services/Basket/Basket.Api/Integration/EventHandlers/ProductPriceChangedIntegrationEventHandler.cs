namespace Basket.Api.IntegrationEvents.EventHandlers
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly IBasketRepository repository;
        private readonly ILogger<ProductPriceChangedIntegrationEventHandler> logger;

        public ProductPriceChangedIntegrationEventHandler(
            IBasketRepository repository,
            ILogger<ProductPriceChangedIntegrationEventHandler> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                logger.LogInformation("[Basket] ---> Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                var userIds = repository.GetCustomers();

                foreach (string id in userIds)
                {
                    var basket = await repository.GetBasketAsync(id);

                    await UpdatePriceInBasketItems(@event.ProductId, @event.NewPrice, @event.OldPrice, basket);
                }
            }
        }

        private async Task UpdatePriceInBasketItems(int productId, decimal newPrice, decimal oldPrice, CustomerBasket? basket)
        {
            var itemsToUpdate = basket is not null ? basket?.Items?.Where(x => x.ProductId == productId).ToList() : new List<BasketItem>();

            if (itemsToUpdate is not null and { Count: > 0 })
            {
                logger.LogInformation("[Basket] ---> ProductPriceChangedIntegrationEventHandler - Updating items in basket for user: {BuyerId} ({@Items})", basket?.CustomerId, itemsToUpdate);

                foreach (var item in itemsToUpdate)
                {
                    if (item.UnitPrice == oldPrice)
                    {
                        var originalPrice = item.UnitPrice;
                        item.UnitPrice = newPrice;
                        item.OldUnitPrice = originalPrice;
                    }
                }
                await repository.UpdateBasketAsync(basket!);
            }
        }
    }
}