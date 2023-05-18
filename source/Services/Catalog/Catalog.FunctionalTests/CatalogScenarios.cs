namespace Catalog.FunctionalTests
{
    public class CatalogScenarios
    {
        [Fact]
        public async Task GetAllItems_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.Items());

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAllBrands_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.Brands());

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAllTypes_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.Types());

            // Then
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task GetItemsByType_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemsByType(1));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetItemsByBrand_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemsByBrand(1));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetItemsByName_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemsByName("Sneakers"));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetItemsByTypeAndBrand_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemsByTypeAndBrand(1, 1));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetItemById_CorrectId_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemById(1));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetItemById_WrongId_ResponseStatusCodeShouldBeBadRequest()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemById(-1));

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetItemById_NonExistentItemId_ResponseStatusCodeShouldBeBadRequest()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.GetAsync(ApiLinks.ItemById(int.MaxValue));

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateItem_CorrectDto_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            var createDto = new CatalogItemCreateDto()
            {
                Name = "requestreq",
                Description = "requestrequestrequestrequestre",
                Price = 100M,
                Discount = 0M,
                CatalogBrandId = 1,
                CatalogTypeId = 1,
                AvailableInStock = 10,
                RestockThreshold = 3,
                MaxStockThreshold = 25,
                OnReorder = false
            };
            var content = JsonContent.Create<CatalogItemCreateDto>(createDto);

            // When
            var response = await client.PostAsync(ApiLinks.Create(), content);

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CreateItem_WrongDto_ResponseStatusShouldBeBadRequest()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            var createDto = new CatalogItemCreateDto();
            var content = JsonContent.Create<CatalogItemCreateDto>(createDto);

            // When
            var response = await client.PostAsync(ApiLinks.Create(), content);

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateItem_CorrectDto_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            var createDto = new CatalogItemUpdateDto()
            {
                Id = 2,
                Name = "requestreq",
                Description = "requestrequestrequestrequestre",
                Price = 100M,
                Discount = 0M,
                CatalogBrandId = 1,
                CatalogTypeId = 1,
                AvailableInStock = 10,
                RestockThreshold = 3,
                MaxStockThreshold = 25,
                OnReorder = false
            };
            var content = JsonContent.Create<CatalogItemUpdateDto>(createDto);

            // When
            var response = await client.PutAsync(ApiLinks.Update(), content);

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateItem_WrongDto_ResponseStatusCodeShouldBeBadRequest()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            var createDto = new CatalogItemUpdateDto();
            var content = JsonContent.Create<CatalogItemUpdateDto>(createDto);

            // When
            var response = await client.PutAsync(ApiLinks.Update(), content);

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateItem_WrongItemId_ResponseStatusCodeShouldBeNotFound()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            var createDto = new CatalogItemUpdateDto()
            {
                Id = int.MaxValue,
                Name = "requestreq",
                Description = "requestrequestrequestrequestre",
                Price = 100M,
                Discount = 0M,
                CatalogBrandId = 1,
                CatalogTypeId = 1,
                AvailableInStock = 10,
                RestockThreshold = 3,
                MaxStockThreshold = 25,
                OnReorder = false
            };
            var content = JsonContent.Create<CatalogItemUpdateDto>(createDto);

            // When
            var response = await client.PutAsync(ApiLinks.Update(), content);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteItem_CorrectItemid_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.DeleteAsync(ApiLinks.Delete(3));

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteItem_WrongItemId_ResponseStatusCodeShouldBeNotFound()
        {
            // Given
            using var client = new CatalogWebHostFactory().CreateClient();

            // When
            var response = await client.DeleteAsync(ApiLinks.Delete(int.MaxValue));

            // Then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}