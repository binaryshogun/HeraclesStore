namespace Ordering.Api.Application.Commands.Records
{
    public record OrderDraftDto
    {
        public IEnumerable<OrderItemDto> OrderItems { get; init; } = default!;
        public decimal Total { get; init; }

        public static OrderDraftDto FromOrder(Order order)
        {
            return new OrderDraftDto()
            {
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Discount = oi.Discount,
                    ProductId = oi.ProductId,
                    UnitPrice = oi.UnitPrice,
                    PictureUrl = oi.PictureUrl,
                    Units = oi.Units,
                    ProductName = oi.ProductName
                }),
                Total = order.Total
            };
        }
    }
}