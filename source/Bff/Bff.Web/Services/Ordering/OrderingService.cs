namespace Bff.Web.Services.Ordering
{
    public class OrderingService : IOrderingService
    {
        private readonly GrpcOrdering.GrpcOrderingClient orderingClient;
        private readonly ILogger<OrderingService> logger;

        public OrderingService(GrpcOrdering.GrpcOrderingClient orderingClient, ILogger<OrderingService> logger)
        {
            this.orderingClient = orderingClient;
            this.logger = logger;
        }

        public async Task<OrderDraftDto?> GetOrderDraftAsync(CustomerBasketDto basketData)
        {
            logger.LogDebug("grpc client created, basketData={@basketData}", basketData);

            var command = MapToOrderDraftRequest(basketData);
            var response = await orderingClient.CreateOrderDraftAsync(command);
            logger.LogDebug("grpc response: {@response}", response);

            return MapToResponse(response, basketData);
        }

        private OrderDraftDto? MapToResponse(OrderDraftDTO orderDraft, CustomerBasketDto basketData)
        {
            if (orderDraft is null)
            {
                return null;
            }

            var data = new OrderDraftDto
            {
                Buyer = basketData.BuyerId,
                Total = (decimal)orderDraft.Total,
            };

            orderDraft.OrderItems.ToList().ForEach(o => data.OrderItems.Add(new OrderItemDto
            {
                Discount = (decimal)o.Discount,
                PictureUrl = o.PictureUrl,
                ProductId = o.ProductId,
                ProductName = o.ProductName,
                UnitPrice = (decimal)o.UnitPrice,
                Units = o.Units,
            }));

            return data;
        }

        private CreateOrderDraftRequest MapToOrderDraftRequest(CustomerBasketDto basketData)
        {
            var command = new CreateOrderDraftRequest
            {
                BuyerId = basketData.BuyerId,
            };

            basketData.Items.ForEach(i => command.Items.Add(new BasketItemDTO
            {
                Id = i.Id.ToString(),
                OldUnitPrice = (double)i.OldUnitPrice,
                PictureUrl = i.PictureUrl,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = (double)i.UnitPrice,
            }));

            return command;
        }
    }
}