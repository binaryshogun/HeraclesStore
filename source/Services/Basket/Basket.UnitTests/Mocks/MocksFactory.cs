namespace Basket.UnitTests.Mocks
{
    public static class MocksFactory
    {
        public static Mock<IBasketRepository> CreateBasketRepositoryMock(Dictionary<string, CustomerBasket> basketsData)
        {
            var repository = new Mock<IBasketRepository>();

            repository.Setup(r => r.GetCustomers()).Returns(() => basketsData.Keys);

            repository
                .Setup(r => r.GetBasketAsync(It.IsAny<string>()))
                .Returns((string customerId) => Task.FromResult(basketsData.GetValueOrDefault(customerId)));

            repository
                .Setup(r => r.UpdateBasketAsync(It.IsAny<CustomerBasket>()))
                .Returns((CustomerBasket basket) =>
                {
                    if (string.IsNullOrEmpty(basket.CustomerId))
                    {
                        return Task.FromResult<CustomerBasket?>(null);
                    }

                    basketsData[basket.CustomerId] = basket;
                    return Task.FromResult<CustomerBasket?>(basket);
                });

            repository
                .Setup(r => r.DeleteBasketAsync(It.IsAny<string>()))
                .Returns((string customerId) => Task.FromResult(basketsData.Remove(customerId)));

            return repository;
        }

        public static Mock<ILogger<T>> CreateLoggerMock<T>()
        {
            var logger = new Mock<ILogger<T>>();

            logger.Setup(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ));

            return logger;
        }
    }
}