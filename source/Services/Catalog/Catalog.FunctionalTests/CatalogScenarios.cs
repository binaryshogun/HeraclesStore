namespace Catalog.FunctionalTests
{
    public class CatalogScenarios
    {
        [Fact]
        public async void GetAllItems_ResponseShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.Items());

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void GetAllBrands_ResponseShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.Brands());

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void GetAllTypes_ResponseShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.Types());

            // Then
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async void GetItemsByType_ResponseShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemsByType(1));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void GetItemsByBrand_ResponseShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemsByBrand(1));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void GetItemsByName_ResponseShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemsByName("Sneakers"));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void GetItemsByTypeAndBrand_ResponseShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemsByTypeAndBrand(1, 1));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void GetItemById_CorrectId_ResponseShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemById(1));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void GetItemById_WrongId_ResponseStatusCodeShouldBeBadRequest()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemById(-1));

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void GetItemById_NonExistentItemId_ResponseStatusCodeShouldBeBadRequest()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemById(int.MaxValue));

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}