namespace Basket.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository repository;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository repository, ILogger<BasketController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync()
        {
            var customerId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (customerId is null)
            {
                return Unauthorized("Bad token");
            }

            var basket = await repository.GetBasketAsync(customerId);

            return basket ?? new CustomerBasket(customerId);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<ActionResult> CheckoutAsync(BasketCheckout checkoutInfo, [FromHeader(Name = "x-requestid")] Guid requestId)
        {
            var customerId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (customerId is null)
            {
                return Unauthorized("Bad token");
            }

            var basket = await repository.GetBasketAsync(customerId);

            if (basket is null)
            {
                return BadRequest("Error occurred while processing request");
            }

            // TODO: Checkout logic with integration events (via Ordering microservice)

            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync(CustomerBasket basket)
        {
            var result = await repository.UpdateBasketAsync(basket);

            if (result is null)
            {
                return BadRequest("Error occurred while processing request");
            }

            return result;
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<ActionResult> DeleteBasketAsync()
        {
            var customerId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (customerId is null)
            {
                return Unauthorized("Bad token");
            }

            var result = await repository.DeleteBasketAsync(customerId);

            return result ? Ok() : BadRequest("Error occurred while processing request");
        }
    }
}