namespace Bff.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;

        public BasketController(ICatalogService catalogService, IBasketService basketService)
        {
            this.catalogService = catalogService;
            this.basketService = basketService;
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<CustomerBasketDto>> GetBasketAsync(string id)
        {
            return await this.basketService.GetByIdAsync(id) ?? new CustomerBasketDto();
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        public async Task<ActionResult<CustomerBasketDto?>> UpdateBasketAsync(UpdateBasketRequest request)
        {
            if (request.Items is null || !request.Items.Any())
            {
                return BadRequest("Need at least one basket item");
            }

            var basket = await basketService.GetByIdAsync(request.BuyerId) ?? new CustomerBasketDto(request.BuyerId);
            var catalogItems = await catalogService.GetCatalogItemsAsync(request.Items.Select(x => x.ProductId));

            var itemsCalculated = request.Items
                    .GroupBy(x => x.ProductId, x => x, (k, i) => new { productId = k, items = i })
                    .Select(groupedItem =>
                    {
                        var item = groupedItem.items.First();
                        item.Quantity = groupedItem.items.Sum(i => i.Quantity);
                        return item;
                    });

            foreach (BasketItemCreateDto item in itemsCalculated)
            {
                var catalogItem = catalogItems.SingleOrDefault(ci => ci.Id == item.ProductId);
                if (catalogItem is null)
                {
                    return BadRequest($"Basket refers to a non-existing catalog item ({item.ProductId})");
                }

                var itemInBasket = basket.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (itemInBasket is null)
                {
                    basket.Items.Add(new BasketItemReadDto()
                    {
                        Id = item.Id,
                        ProductId = catalogItem.Id,
                        ProductName = catalogItem.Name,
                        PictureUrl = catalogItem.PictureUrl,
                        UnitPrice = catalogItem.Price,
                        Quantity = item.Quantity
                    });
                }
                else
                {
                    itemInBasket.Quantity = item.Quantity;
                }
            }

            basket.Items.RemoveAll(i => !itemsCalculated.ToList().Exists(item => i.Id == item.Id));

            await this.basketService.UpdateAsync(basket);

            return await this.basketService.GetByIdAsync(basket.BuyerId);
        }

        [HttpPut("items")]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        public async Task<ActionResult<CustomerBasketDto>> UpdateQuantitiesAsync(UpdateBasketItemsRequest request)
        {
            if (!request.UpdatedItems.Any())
            {
                return BadRequest("Need at least one item to update");
            }

            var currentBasket = await basketService.GetByIdAsync(request.BuyerId);
            if (currentBasket is null)
            {
                return BadRequest($"Basket with id {request.BuyerId} not found.");
            }

            foreach (BasketItemUpdateDto item in request.UpdatedItems)
            {
                var basketItem = currentBasket.Items.SingleOrDefault(bitem => bitem.Id == item.BasketItemId);
                if (basketItem is null)
                {
                    return BadRequest($"Basket item with id {item.BasketItemId} not found");
                }
                basketItem.Quantity = item.Quantity;
            }

            await basketService.UpdateAsync(currentBasket);

            return currentBasket;
        }

        [HttpPost("items")]
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        public async Task<ActionResult> AddBasketItemAsync(AddBasketItemRequest request)
        {
            if (request is null || request.Quantity <= 0)
            {
                return BadRequest("Invalid data");
            }

            var item = await catalogService.GetCatalogItemAsync(request.CatalogItemId);

            var currentBasket = (await basketService.GetByIdAsync(request.BasketId)) ?? new CustomerBasketDto(request.BasketId);

            var product = currentBasket.Items.SingleOrDefault(i => i.ProductId == item.Id);
            if (product is not null)
            {
                product.Quantity += request.Quantity;
            }
            else
            {
                currentBasket.Items.Add(new BasketItemReadDto()
                {
                    UnitPrice = item.Price,
                    PictureUrl = item.PictureUrl,
                    ProductId = item.Id,
                    ProductName = item.Name,
                    Quantity = request.Quantity,
                    Id = Guid.NewGuid()
                });
            }

            await basketService.UpdateAsync(currentBasket);

            return Ok();
        }
    }
}