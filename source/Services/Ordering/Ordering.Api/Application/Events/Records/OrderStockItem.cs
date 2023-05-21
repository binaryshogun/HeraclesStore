namespace Ordering.Api.Application.Events.Records
{
    public record OrderStockItem
    {
        public OrderStockItem(int productId, int units)
        {
            ProductId = productId;
            Units = units;
        }

        public int ProductId { get; private set; }
        public int Units { get; private set; }
    }
}