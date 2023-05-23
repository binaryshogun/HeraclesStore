namespace Bff.Web.Services.Basket
{
    public class BasketService : IBasketService
    {
        private readonly GrpcBasket.GrpcBasketClient basketClient;
        private readonly ILogger<BasketService> logger;

        public BasketService(GrpcBasket.GrpcBasketClient basketClient, ILogger<BasketService> logger)
        {
            this.basketClient = basketClient;
            this.logger = logger;
        }

        public async Task<CustomerBasketDto?> GetByIdAsync(string? buyerId)
        {
            logger.LogDebug("Grpc client created, request = {@id}", buyerId);
            var response = await basketClient.GetBasketByIdAsync(new BasketRequest { Id = buyerId });
            logger.LogDebug("grpc response {@response}", response);

            return MapToBasketData(response);
        }

        public async Task UpdateAsync(CustomerBasketDto basket)
        {
            logger.LogDebug("Grpc update basket currentBasket {@currentBasket}", basket);
            var request = MapToCustomerBasketRequest(basket);
            logger.LogDebug("Grpc update basket request {@request}", request);

            await basketClient.UpdateBasketAsync(request);
        }

        private CustomerBasketDto? MapToBasketData(CustomerBasketResponse customerBasketRequest)
        {
            if (customerBasketRequest is null)
            {
                return null;
            }

            var map = new CustomerBasketDto
            {
                BuyerId = customerBasketRequest.BuyerId
            };

            customerBasketRequest.Items.ToList().ForEach(item =>
            {
                if (item.Id is not null)
                {
                    var id = Guid.Parse(item.Id);

                    if (id != Guid.Empty)
                    {
                        map.Items.Add(new BasketItemReadDto
                        {
                            Id = id,
                            OldUnitPrice = (decimal)item.OldUnitPrice,
                            PictureUrl = item.PictureUrl,
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            Quantity = item.Quantity,
                            UnitPrice = (decimal)item.UnitPrice
                        });
                    }
                }
            });

            return map;
        }

        private CustomerBasketRequest? MapToCustomerBasketRequest(CustomerBasketDto basketData)
        {
            if (basketData is null)
            {
                return null;
            }

            var map = new CustomerBasketRequest
            {
                BuyerId = basketData.BuyerId
            };

            basketData.Items.ToList().ForEach(item =>
            {
                if (item.Id != Guid.Empty)
                {
                    map.Items.Add(new CustomerBasketItem
                    {
                        Id = item.Id.ToString(),
                        OldUnitPrice = (double)item.OldUnitPrice,
                        PictureUrl = item.PictureUrl,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = (double)item.UnitPrice
                    });
                }
            });

            return map;
        }
    }
}