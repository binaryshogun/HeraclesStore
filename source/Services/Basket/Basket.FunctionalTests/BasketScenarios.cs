namespace Basket.FunctionalTests
{
    public class BasketScenarios
    {
        [Fact]
        public async Task GetBasketByIdAsync_ResponseStatusShouldBeSuccess()
        {
            // Given
            using var client = new BasketWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.Api);

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CheckoutAsync_ResponseStatusShouldBeSuccess()
        {
            // Given
            using var client = new BasketWebHostFactory().CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());

            var checkoutInfo = new BasketCheckout()
            {
                City = "City",
                Street = "Street",
                State = "State",
                ZipCode = "123456",
                CardHolder = "Firstname Lastname",
                CardNumber = "1234567890123456",
                CardSecurityNumber = "123",
                CardTypeId = 1,
                CardExpiration = DateTime.Now.AddYears(2),
            };

            var content = JsonContent.Create<BasketCheckout>(checkoutInfo);

            // Get new basket for current customer and update it on server
            await client.PutAsync(ApiLinks.Api, (await client.GetAsync(ApiLinks.Api)).Content);

            // When
            var response = await client.PostAsync(ApiLinks.Api, content);

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CheckoutAsync_NoRequestHeader_ResponseStatusShouldBeBadRequest()
        {
            // Given
            using var client = new BasketWebHostFactory().CreateClient();

            var checkoutInfo = new BasketCheckout()
            {
                City = "City",
                Street = "Street",
                State = "State",
                ZipCode = "123456",
                CardHolder = "Firstname Lastname",
                CardNumber = "1234567890123456",
                CardSecurityNumber = "123",
                CardTypeId = 1,
                CardExpiration = DateTime.Now.AddYears(2),
            };

            var content = JsonContent.Create<BasketCheckout>(checkoutInfo);

            // Get new basket for current customer and update it on server
            await client.PutAsync(ApiLinks.Api, (await client.GetAsync(ApiLinks.Api)).Content);

            // When
            var response = await client.PostAsync(ApiLinks.Api, content);

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CheckoutAsync_WrongModel_ResponseStatusShouldBeBadRequest()
        {
            // Given
            using var client = new BasketWebHostFactory().CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());

            var checkoutInfo = new BasketCheckout()
            {
                City = "City",
                State = "State",
                ZipCode = "123456",
                CardHolder = "Firstname Lastname",
                CardNumber = "1234567890123456",
                CardSecurityNumber = "123",
                CardTypeId = 1,
                CardExpiration = DateTime.Now.AddYears(2),
            };

            var content = JsonContent.Create<BasketCheckout>(checkoutInfo);

            // Get new basket for current customer and update it on server
            await client.PutAsync(ApiLinks.Api, (await client.GetAsync(ApiLinks.Api)).Content);

            // When
            var response = await client.PostAsync(ApiLinks.Api, content);

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBasketAsync_ResponseStatusShouldBeSuccess()
        {
            // Given
            using var client = new BasketWebHostFactory().CreateClient();

            // When:
            // Get new basket for current customer and update it on server
            var response = await client.PutAsync(ApiLinks.Api, (await client.GetAsync(ApiLinks.Api)).Content);

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteBasketAsync_ResponseStatusShouldBeSuccess()
        {
            // Given
            using var client = new BasketWebHostFactory().CreateClient();

            // Get new basket for current customer and update it on server
            await client.PutAsync(ApiLinks.Api, (await client.GetAsync(ApiLinks.Api)).Content);

            // When:
            var response = await client.DeleteAsync(ApiLinks.Api);

            // Then
            response.EnsureSuccessStatusCode();
        }
    }
}