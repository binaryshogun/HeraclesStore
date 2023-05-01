namespace Basket.UnitTests.API
{
    public class BasketControllerTests
    {
        private readonly Mock<IBasketRepository> repository;
        private readonly Mock<ILogger<BasketController>> logger;

        public BasketControllerTests()
        {
            repository = MocksFactory.CreateBasketRepositoryMock(GetBasketsData());
            logger = MocksFactory.CreateLoggerMock<BasketController>();
        }

        [Fact]
        public async Task GetBasketByIdAsync_CustomerIdDefined_ResponseShouldBeStatus200OK()
        {
            // Given
            const string customerId = "customer1";

            var controller = new BasketController(repository.Object, logger.Object)
            {
                ControllerContext = CreateFakeControllerContext(customerId)
            };

            // When
            var response = await controller.GetBasketByIdAsync();

            // Then
            var actionResult = Assert.IsType<ActionResult<CustomerBasket>>(response);

            var basket = actionResult.Value;
            Assert.NotNull(basket);
            Assert.Equal(customerId, basket.CustomerId);
            Assert.Equal(GetDefaultBasketItems().Count, basket.Items.Count);

            repository.Verify(r => r.GetBasketAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetBasketByIdAsync_CustomerIdNotDefined_ResponseShouldBeStatus401Unauthorized()
        {
            // Given
            const string? customerId = null;

            var controller = new BasketController(repository.Object, logger.Object)
            {
                ControllerContext = CreateFakeControllerContext(customerId)
            };

            // When
            var response = await controller.GetBasketByIdAsync();

            // Then
            var actionResult = Assert.IsType<ActionResult<CustomerBasket>>(response);
            Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);

            var basket = actionResult.Value;
            Assert.Null(basket);

            repository.Verify(r => r.GetBasketAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CheckoutAsync_CustomerIdDefined_CorrectCustomerId_ResponseShouldBeStatus200OK()
        {
            // Given
            const string? customerId = "customer1";

            var controller = new BasketController(repository.Object, logger.Object)
            {
                ControllerContext = CreateFakeControllerContext(customerId)
            };

            var checkoutInfo = new BasketCheckout()
            {
                City = "City",
                Street = "Street",
                State = "State",
                ZipCode = "123456",
                CardNumber = "1234567890123456",
                CardHolder = "Firstname Surname",
                CardExpiration = DateTime.Now.AddYears(2),
                CardSecurityNumber = "123",
                CardTypeId = 1
            };

            // When
            var response = await controller.CheckoutAsync(checkoutInfo, Guid.NewGuid());

            // Then
            var result = Assert.IsType<OkResult>(response);
            repository.Verify(r => r.GetBasketAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CheckoutAsync_CustomerIdDefined_WrongCustomerId_ResponseShouldBeStatus400BadRequest()
        {
            // Given
            const string? customerId = "wrongcustomer";

            var controller = new BasketController(repository.Object, logger.Object)
            {
                ControllerContext = CreateFakeControllerContext(customerId)
            };

            var checkoutInfo = new BasketCheckout()
            {
                City = "City",
                Street = "Street",
                State = "State",
                ZipCode = "123456",
                CardNumber = "1234567890123456",
                CardHolder = "Firstname Surname",
                CardExpiration = DateTime.Now.AddYears(2),
                CardSecurityNumber = "123",
                CardTypeId = 1
            };

            // When
            var response = await controller.CheckoutAsync(checkoutInfo, Guid.NewGuid());

            // Then
            var result = Assert.IsType<BadRequestObjectResult>(response);
            repository.Verify(r => r.GetBasketAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CheckoutAsync_CustomerIdNotDefined_ResponseShouldBeStatus401Unauthorized()
        {
            // Given
            const string? customerId = null;

            var controller = new BasketController(repository.Object, logger.Object)
            {
                ControllerContext = CreateFakeControllerContext(customerId)
            };

            var checkoutInfo = new BasketCheckout()
            {
                City = "City",
                Street = "Street",
                State = "State",
                ZipCode = "123456",
                CardNumber = "1234567890123456",
                CardHolder = "Firstname Surname",
                CardExpiration = DateTime.Now.AddYears(2),
                CardSecurityNumber = "123",
                CardTypeId = 1
            };

            // When
            var response = await controller.CheckoutAsync(checkoutInfo, Guid.NewGuid());

            // Then
            var result = Assert.IsType<UnauthorizedObjectResult>(response);
            repository.Verify(r => r.GetBasketAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBasketAsync_CorrectBasket_ResponseShouldBeStatus200OK()
        {
            // Given
            const string customerId = "customer1";
            var basket = await repository.Object.GetBasketAsync(customerId) ?? new CustomerBasket()
            {
                CustomerId = customerId,
                Items = GetDefaultBasketItems()
            };

            basket.Items.RemoveAt(2);

            var controller = new BasketController(repository.Object, logger.Object);

            // When
            var response = await controller.UpdateBasketAsync(basket);

            // Then
            var actionResult = Assert.IsType<ActionResult<CustomerBasket>>(response);

            basket = actionResult.Value;
            Assert.NotNull(basket);
            Assert.Equal(customerId, basket.CustomerId);
            Assert.Equal(2, basket.Items.Count);

            repository.Verify(r => r.UpdateBasketAsync(It.IsAny<CustomerBasket>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBasketAsync_WrongBasket_ResponseShouldBeStatus400BadRequest()
        {
            // Given
            var basket = new CustomerBasket();
            var controller = new BasketController(repository.Object, logger.Object);

            // When
            var response = await controller.UpdateBasketAsync(basket);

            // Then
            var actionResult = Assert.IsType<ActionResult<CustomerBasket>>(response);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Null(actionResult.Value);

            repository.Verify(r => r.UpdateBasketAsync(It.IsAny<CustomerBasket>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBasketAsync_CustomerIdDefined_BasketWithCustomerIdExists_ResponseShouldBeStatus200OK()
        {
            // Given
            const string customerId = "customer1";

            var controller = new BasketController(repository.Object, logger.Object)
            {
                ControllerContext = CreateFakeControllerContext(customerId)
            };

            // When
            var response = await controller.DeleteBasketAsync();

            // Then
            var actionResult = Assert.IsType<OkResult>(response);
            repository.Verify(r => r.DeleteBasketAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBasketAsync_CustomerIdDefined_BasketWithCustomerIdNotExists_ResponseShouldBeStatus400BadRequest()
        {
            // Given
            const string customerId = "customerwithoutbasket";

            var controller = new BasketController(repository.Object, logger.Object)
            {
                ControllerContext = CreateFakeControllerContext(customerId)
            };

            // When
            var response = await controller.DeleteBasketAsync();

            // Then
            var actionResult = Assert.IsType<BadRequestObjectResult>(response);
            repository.Verify(r => r.DeleteBasketAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBasketAsync_CustomerIdNotDefined_ResponseShouldBeStatus401Unauthorized()
        {
            // Given
            const string? customerId = null;

            var controller = new BasketController(repository.Object, logger.Object)
            {
                ControllerContext = CreateFakeControllerContext(customerId)
            };

            // When
            var response = await controller.DeleteBasketAsync();

            // Then
            var actionResult = Assert.IsType<UnauthorizedObjectResult>(response);
            repository.Verify(r => r.DeleteBasketAsync(It.IsAny<string>()), Times.Never);
        }

        private ControllerContext CreateFakeControllerContext(string? customerId)
        {
            Claim? claim = null;

            if (customerId is not null)
            {
                claim = new Claim(ClaimTypes.NameIdentifier, customerId);
            }

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(u => u.FindFirst(It.IsAny<string>())).Returns((string type) => claim);

            var httpContext = Mock.Of<HttpContext>(c => c.User == user.Object);

            return Mock.Of<ControllerContext>(c => c.HttpContext == httpContext);
        }

        private Dictionary<string, CustomerBasket> GetBasketsData()
        {
            const string customerName = "customer";
            int customerId = 1;

            return new()
            {
                [$"{customerName}{customerId}"] = new CustomerBasket()
                {
                    CustomerId = $"{customerName}{customerId++}",
                    Items = GetDefaultBasketItems()
                },
                [$"{customerName}{customerId}"] = new CustomerBasket()
                {
                    CustomerId = $"{customerName}{customerId++}",
                    Items = GetDefaultBasketItems()
                },
                [$"{customerName}{customerId}"] = new CustomerBasket()
                {
                    CustomerId = $"{customerName}{customerId++}",
                    Items = GetDefaultBasketItems()
                },
            };
        }

        private List<BasketItem> GetDefaultBasketItems()
        {
            return new()
            {
                new BasketItem()
                {
                    Id = 1,
                    ProductId = 1,
                    ProductName = "Product 3",
                    UnitPrice = 100,
                    OldUnitPrice = 100,
                    Quantity = 1,
                },
                new BasketItem()
                {
                    Id = 2,
                    ProductId = 2,
                    ProductName = "Product 2",
                    UnitPrice = 100,
                    OldUnitPrice = 100,
                    Quantity = 2,
                },
                new BasketItem()
                {
                    Id = 3,
                    ProductId = 3,
                    ProductName = "Product 3",
                    UnitPrice = 100,
                    OldUnitPrice = 100,
                    Quantity = 3,
                },
            };
        }
    }
}