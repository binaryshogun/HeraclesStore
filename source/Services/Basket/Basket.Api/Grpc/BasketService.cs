namespace Basket.Api.Grpc
{
    public class BasketService : GrpcBasket.GrpcBasketBase
    {
        private readonly IBasketRepository repository;
        private readonly ILogger<BasketService> logger;

        public BasketService(IBasketRepository repository, ILogger<BasketService> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public override async Task<CustomerBasketResponse> GetBasketById(BasketRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call from method {Method} for basket id {Id}", context.Method, request.Id);

            var data = await repository.GetBasketAsync(request.Id);

            if (data != null)
            {
                context.Status = new Status(StatusCode.OK, $"Basket with given id ({request.Id}) do exist");

                return MapToCustomerBasketResponse(data);
            }
            else
            {
                context.Status = new Status(StatusCode.NotFound, $"Basket with given id ({request.Id}) do not exist");
            }

            return new CustomerBasketResponse();
        }

        public override async Task<CustomerBasketResponse?> UpdateBasket(CustomerBasketRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call BasketService.UpdateBasketAsync for buyer id {Buyerid}", request.BuyerId);

            var customerBasket = MapToCustomerBasket(request);

            var response = await repository.UpdateBasketAsync(customerBasket);

            if (response != null)
            {
                return MapToCustomerBasketResponse(response);
            }

            context.Status = new Status(StatusCode.NotFound, $"Basket with buyer id {request.BuyerId} do not exist");

            return null;
        }

        private CustomerBasketResponse MapToCustomerBasketResponse(CustomerBasket customerBasket)
        {
            var response = new CustomerBasketResponse
            {
                BuyerId = customerBasket.CustomerId
            };

            customerBasket.Items.ForEach(item => response.Items.Add(new CustomerBasketItem
            {
                Id = item.Id,
                ProductId = item.ProductId,
                UnitPrice = (double)item.UnitPrice,
                OldUnitPrice = (double)item.OldUnitPrice,
                Quantity = item.Quantity,
                PictureUrl = item.PictureUrl,
                ProductName = item.ProductName,
            }));

            return response;
        }

        private CustomerBasket MapToCustomerBasket(CustomerBasketRequest customerBasketRequest)
        {
            var response = new CustomerBasket
            {
                CustomerId = customerBasketRequest.BuyerId
            };

            customerBasketRequest.Items.ToList().ForEach(item => response.Items.Add(new BasketItem
            {
                Id = item.Id,
                OldUnitPrice = (decimal)item.OldUnitPrice,
                PictureUrl = item.PictureUrl,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = (decimal)item.UnitPrice
            }));

            return response;
        }
    }
}