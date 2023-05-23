namespace Ordering.Api.Grpc
{
    public class OrderingService : GrpcOrdering.GrpcOrderingBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<OrderingService> logger;

        public OrderingService(IMediator mediator, ILogger<OrderingService> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public override async Task<OrderDraftDTO> CreateOrderDraft(CreateOrderDraftRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call from method {Method} for ordering get order draft {CreateOrderDraftCommand}", context.Method, request);
            logger.LogTrace("Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})", request.GetGenericTypeName(), nameof(request.BuyerId), request.BuyerId, request);

            var command = new CreateOrderDraftCommand(request.BuyerId, MapBasketItems(request.Items));

            var response = await mediator.Send(command);

            if (response != null)
            {
                context.Status = new Status(StatusCode.OK, $" ordering get order draft {request} do exist");

                return MapResponse(response);
            }
            else
            {
                context.Status = new Status(StatusCode.NotFound, $" ordering get order draft {request} do not exist");
            }

            return new OrderDraftDTO();
        }

        public OrderDraftDTO MapResponse(OrderDraftDto order)
        {
            var result = new OrderDraftDTO()
            {
                Total = (double)order.Total,
            };

            order.OrderItems.ToList().ForEach(i => result.OrderItems.Add(new OrderItemDTO()
            {
                Discount = (double)i.Discount,
                PictureUrl = i.PictureUrl,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = (double)i.UnitPrice,
                Units = i.Units,
            }));

            return result;
        }

        public IEnumerable<BasketItem> MapBasketItems(RepeatedField<CustomerBasketItem> items)
        {
            return items.Select(x => new BasketItem()
            {
                Id = Guid.Parse(x.Id),
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                UnitPrice = (decimal)x.UnitPrice,
                OldUnitPrice = (decimal)x.OldUnitPrice,
                Quantity = x.Quantity,
                PictureUrl = x.PictureUrl,
            });
        }
    }
}