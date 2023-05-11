namespace Ordering.FunctionalTests
{
    public class OrderingScenarios
    {
        [Fact]
        public async Task GetOrders_ResponseStatusCodeShouldBeStatus200OK()
        {
            // Given
            using var client = new OrderingWebApplicationFactory().CreateIdempotentClient();

            // When
            var response = await client.GetAsync(ApiLinks.GetOrders());

            // Then
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CancelOrder_WrongOrder_ResponseStatusShouldBeStatus404NotFound()
        {
            // Given
            using var client = new OrderingWebApplicationFactory().CreateIdempotentClient();
            var content = JsonContent.Create(new CancelOrderCommand(-1));

            // When
            var response = await client.PutAsync(ApiLinks.CancelOrder(), content);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ShipOrder_WrongOrder_ResponseStatusShouldBeStatus404NotFound()
        {
            // Given
            using var client = new OrderingWebApplicationFactory().CreateIdempotentClient();
            var content = JsonContent.Create(new ShipOrderCommand(-1));

            // When
            var response = await client.PutAsync(ApiLinks.ShipOrder(), content);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}