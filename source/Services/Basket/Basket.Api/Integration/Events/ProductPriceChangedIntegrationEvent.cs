namespace Basket.Api.IntegrationEvents.Events
{
    public record ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public ProductPriceChangedIntegrationEvent(int productId, decimal newPrice, decimal oldPrice)
        {
            ProductId = productId;
            NewPrice = newPrice;
            OldPrice = oldPrice;
        }

        public int ProductId { get; private init; }
        public decimal NewPrice { get; private init; }
        public decimal OldPrice { get; private init; }
    }
}