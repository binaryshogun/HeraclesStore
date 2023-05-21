namespace Basket.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository repository;
        private readonly IEventBus eventBus;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository repository, IEventBus eventBus, ILogger<BasketController> logger)
        {
            this.repository = repository;
            this.eventBus = eventBus;
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
            if (requestId == Guid.Empty)
            {
                return BadRequest("Empty request ID");
            }

            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (customerId is null)
            {
                return Unauthorized("Bad token");
            }

            var basket = await repository.GetBasketAsync(customerId);

            if (basket is null)
            {
                return BadRequest("Error occurred while processing request");
            }

            var userId = Guid.Parse(customerId);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            if (userId == Guid.Empty || string.IsNullOrEmpty(userName))
            {
                return BadRequest("Wrong user credentials");
            }

            var integrationEvent = new UserCheckoutAcceptedIntegrationEvent(userId, userName,
                checkoutInfo.City!, checkoutInfo.Street!, checkoutInfo.State!, checkoutInfo.Country!,
                checkoutInfo.ZipCode!, checkoutInfo.CardNumber!, checkoutInfo.CardHolder!, checkoutInfo.CardExpiration,
                checkoutInfo.CardSecurityNumber!, checkoutInfo.CardTypeId, requestId, basket);

            try
            {
                eventBus.PublishEvent(integrationEvent);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Basket] ---> ERROR Publishing integration event: {IntegrationEventId}", integrationEvent.Id);

                return BadRequest("Error occurred during checkout");
            }

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