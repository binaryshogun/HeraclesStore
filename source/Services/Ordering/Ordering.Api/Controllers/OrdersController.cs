namespace Ordering.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ILogger<OrdersController> logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderSummary>), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null || Guid.Parse(userId) == Guid.Empty)
            {
                return Unauthorized();
            }

            var query = new GetOrdersByUserQuery(Guid.Parse(userId));
            var orders = await mediator.Send(query);

            return Ok(orders);
        }

        [HttpGet("{orderId:int}")]
        [ProducesResponseType(typeof(OrderDetails), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound, "application/json")]
        public async Task<ActionResult<OrderDetails>> GetOrderAsync(int orderId)
        {
            try
            {
                var query = new GetOrderQuery(orderId);

                var order = await mediator.Send(query);

                return order;
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("cardtypes")]
        [ProducesResponseType(typeof(IEnumerable<CardType>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CardTypeSummary>> GetCardTypesAsync()
        {
            var query = new GetCardTypesQuery();

            var cardTypes = await mediator.Send(query);

            return cardTypes;
        }

        [HttpPut("ship")]
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound, "application/json")]
        public async Task<IActionResult> ShipOrderAsync(ShipOrderCommand command, [FromHeader(Name = "x-requestid")] Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                return BadRequest("Request Id should not be empty");
            }

            var request = new IdentifiedCommand<ShipOrderCommand, bool>(command, requestId);

            logger.LogInformation("[Ordering] ---> Sending command {CommandName} with {@Property} parameter: {@Parameter} ({@Command})",
                request.GetType().Name, nameof(command.OrderId), command.OrderId, request);

            var response = await mediator.Send(request);

            if (!response)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("cancel")]
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(BadRequestObjectResult), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound, "application/json")]
        public async Task<IActionResult> CancelOrderAsync(CancelOrderCommand command, [FromHeader(Name = "x-requestid")] Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                return BadRequest("Request Id should not be empty");
            }

            var request = new IdentifiedCommand<CancelOrderCommand, bool>(command, requestId);

            logger.LogInformation("[Ordering] ---> Sending command {CommandName} with {@Property} parameter: {@Parameter} ({@Command})",
                request.GetType().Name, nameof(command.OrderId), command.OrderId, request);

            var response = await mediator.Send(request);

            if (!response)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}