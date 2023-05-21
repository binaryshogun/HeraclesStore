namespace Catalog.Api.Integration.Records
{
    public record ConfirmedOrderStockItem
    {
        public ConfirmedOrderStockItem(int productId, bool hasStock)
        {
            ProductId = productId;
            HasStock = hasStock;
        }

        public int ProductId { get; }
        public bool HasStock { get; }
    }
}