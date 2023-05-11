namespace Ordering.UnitTests.API
{
    public class OrdersControllerTests
    {
        private readonly Mock<IMediator> mediator;
        private readonly Mock<ILogger<OrdersController>> logger;

        public OrdersControllerTests()
        {
            mediator = new Mock<IMediator>();
            logger = new Mock<ILogger<OrdersController>>();
        }

        [Fact]
        public async Task CancelOrder_GivenRequestId_ShouldReturnStatus200OK()
        {
            // Given
            mediator.Setup(m => m.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(true));

            // When
            var controller = new OrdersController(mediator.Object, logger.Object);
            var response = await controller.CancelOrderAsync(new CancelOrderCommand(1), Guid.NewGuid());

            // Then
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task CancelOrder_EmptyRequestId_ShouldReturnStatus400BadRequest()
        {
            // Given
            mediator.Setup(m => m.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(true));

            // When
            var controller = new OrdersController(mediator.Object, logger.Object);
            var response = await controller.CancelOrderAsync(new CancelOrderCommand(1), Guid.Empty);

            // Then
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task CancelOrder_WrongOrderId_ShouldReturnStatus404NotFound()
        {
            // Given
            mediator.Setup(m => m.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(false));

            // When
            var controller = new OrdersController(mediator.Object, logger.Object);
            var response = await controller.CancelOrderAsync(new CancelOrderCommand(1), Guid.NewGuid());

            // Then
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task ShipOrder_ShouldReturnStatus200OK()
        {
            // Given
            mediator.Setup(m => m.Send(It.IsAny<IdentifiedCommand<ShipOrderCommand, bool>>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(true));

            // When
            var controller = new OrdersController(mediator.Object, logger.Object);
            var response = await controller.ShipOrderAsync(new ShipOrderCommand(1), Guid.NewGuid());

            // Then
            Assert.IsType<OkResult>(response);

        }

        [Fact]
        public async Task ShipOrder_WronRequestId_ShouldReturnStatus400BadRequest()
        {
            // Given
            mediator.Setup(m => m.Send(It.IsAny<IdentifiedCommand<ShipOrderCommand, bool>>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(true));

            // When
            var controller = new OrdersController(mediator.Object, logger.Object);
            var response = await controller.ShipOrderAsync(new ShipOrderCommand(1), Guid.Empty);

            // Then
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnStatus200OK()
        {
            // Given
            IEnumerable<OrderSummary> orders = new List<OrderSummary>()
            {
                Mock.Of<OrderSummary>()
            };
            int count = orders.ToList().Count;

            mediator
                .Setup(m => m.Send(It.IsAny<GetOrdersByUserQuery>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(orders));

            // When
            var controller = new OrdersController(mediator.Object, logger.Object)
            {
                ControllerContext = Mock.Of<ControllerContext>(
                    cc => cc.HttpContext == Mock.Of<HttpContext>(
                        hc => hc.User == Mock.Of<ClaimsPrincipal>(
                            u => u.FindFirst(ClaimTypes.NameIdentifier) == new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()))))
            };
            var actionResult = await controller.GetOrdersAsync();

            // Then
            var result = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.NotNull(result.Value);

            var value = Assert.IsType<List<OrderSummary>>(result.Value);
            Assert.Equal(count, value.Count);
        }

        [Fact]
        public async Task GetOrder_ShouldReturnStatus200OK()
        {
            // Given
            const int orderId = 123;
            var order = Mock.Of<OrderDetails>(o => o.Id == orderId);

            mediator
                .Setup(m => m.Send(It.IsAny<GetOrderQuery>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(order));

            // When
            var orderController = new OrdersController(mediator.Object, logger.Object);
            var response = await orderController.GetOrderAsync(orderId);

            // Then
            Assert.NotNull(response.Value);
            Assert.Equal(orderId, response.Value.Id);
        }

        [Fact]
        public async Task GetCardTypes_ShouldReturnStatus200OK()
        {
            // Given
            IEnumerable<CardTypeSummary> cardTypes = new List<CardTypeSummary>()
            {
                Mock.Of<CardTypeSummary>()
            };
            int count = cardTypes.ToList().Count;

            mediator
                .Setup(m => m.Send(It.IsAny<GetCardTypesQuery>(), default(System.Threading.CancellationToken)))
                .Returns(Task.FromResult(cardTypes));

            // When
            var orderController = new OrdersController(mediator.Object, logger.Object);
            var response = await orderController.GetCardTypesAsync();

            // Then
            Assert.NotNull(response);
            Assert.Equal(count, response.Count());
        }
    }
}