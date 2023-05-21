namespace Ordering.Api.Application.Commands
{
    public class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftCommand, OrderDraftDto>
    {
        public Task<OrderDraftDto> Handle(CreateOrderDraftCommand request, CancellationToken cancellationToken)
        {
            var order = Order.NewDraft();

            var orderItems = request.Items.Select(i => new OrderItemDto()
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Discount = i.OldUnitPrice - i.UnitPrice,
                Units = i.Quantity,
                PictureUrl = i.PictureUrl,
            });

            foreach (var item in orderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName!, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }

            return Task.FromResult(OrderDraftDto.FromOrder(order));
        }
    }
}