namespace Catalog.Api.Integration.Records
{
    public record OrderStockItem
    {
        public OrderStockItem(int productId, int units)
        {
            ProductId = productId;
            Units = units;
        }

        public int ProductId { get; }
        public int Units { get; }
    }
}