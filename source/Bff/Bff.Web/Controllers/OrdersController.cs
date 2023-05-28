namespace Bff.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IBasketService basketService;
        private readonly IOrderingService orderingService;

        public OrdersController(IBasketService basketService, IOrderingService orderingService)
        {
            this.basketService = basketService;
            this.orderingService = orderingService;
        }

        [HttpGet("draft/{basketId:Guid}")]
        [ProducesResponseType(typeof(OrderDraftDto), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        public async Task<ActionResult<OrderDraftDto?>> GetOrderDraftAsync(string buyerId)
        {
            if (string.IsNullOrWhiteSpace(buyerId))
            {
                return BadRequest("Need a valid basket identifier");
            }

            var basket = await basketService.GetByIdAsync(buyerId);

            if (basket is null)
            {
                return BadRequest($"No basket found for id {buyerId}");
            }

            return await orderingService.GetOrderDraftAsync(basket);
        }
    }
}