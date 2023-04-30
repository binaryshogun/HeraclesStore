namespace Catalog.UnitTests.API
{
    public class CatalogControllerTests
    {
        private readonly Mock<ICatalogRepository> repository;
        private readonly IMapper controllerMapper;

        public CatalogControllerTests()
        {
            repository = CatalogDataMocks.CreateRepositoryMock(
                GetDefaultCatalogItems(),
                GetDefaultCatalogBrands(),
                GetDefaultCatalogTypes()
            );
            controllerMapper = CreateControllerMapper();
        }

        [Fact]
        public async Task GetItems_ShouldReturnFifteenItems()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var items = await controller.GetItemsAsync();

            // Then
            Assert.Equal(15, items.Count());
            repository.Verify(r => r.GetItemsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task GetItemsByType_SecondType_ShouldReturnThreeItems()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var items = await controller.GetItemsByTypeAsync(2);

            // Then
            Assert.Equal(3, items.Count());
            repository.Verify(r => r.GetItemsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task GetItemsByBrand_FirstBrand_ShouldReturnThreeItems()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var items = await controller.GetItemsByBrandAsync(1);

            // Then
            Assert.Equal(3, items.Count());
            repository.Verify(r => r.GetItemsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task GetItemsByName_NameIncludesSneakers_ShouldReturnThreeItems()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var items = await controller.GetItemsByNameAsync("Sneakers");

            // Then
            Assert.Equal(3, items.Count());
            repository.Verify(r => r.GetItemsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task GetItemsByTypeAndBrand_SecondType_FirstBrand_ShouldReturnOneItem()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var items = await controller.GetItemsByTypeAndBrandAsync(2, 1);

            // Then
            Assert.Single(items);
            repository.Verify(r => r.GetItemsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task GetBrands_ShouldReturnNineBrands()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var brands = await controller.GetBrandsAsync();

            // Then
            Assert.Equal(9, brands.Count());
            repository.Verify(r => r.GetAllBrandsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTypes_ShouldReturnEightTypes()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var types = await controller.GetTypesAsync();

            // Then
            Assert.Equal(8, types.Count());
            repository.Verify(r => r.GetAllTypesAsync(), Times.Once);
        }

        [Fact]
        public void CreateItem_ShouldReturnCreatedAtAction()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);
            var item = new CatalogItemCreateDto() { Name = "New item" };

            // When
            var result = controller.CreateCatalogItem(item);

            // Then
            Assert.IsType<CreatedAtActionResult>(result);
            repository.Verify(r => r.CreateItem(It.IsAny<CatalogItem>()), Times.Once);
            repository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [Fact]
        public async void UpdateCatalogItem_ItemIdEqualsFive_ShouldReturnNoContent()
        {
            // Given
            const string name = "New item";

            var controller = new CatalogController(repository.Object, controllerMapper);
            var item = new CatalogItemUpdateDto() { Id = 5, Name = name };

            // When
            var result = await controller.UpdateCatalogItemAsync(item);

            // Then
            Assert.IsType<NoContentResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            repository.Verify(r => r.UpdateItem(It.IsAny<int>(), It.IsAny<CatalogItem>()), Times.Once);
            repository.Verify(r => r.SaveChangesAsync(), Times.Once);

            var updatedItem = await repository.Object.GetItemByIdAsync(5);
            Assert.NotNull(updatedItem);
            Assert.Equal(name, updatedItem.Name);
        }

        [Fact]
        public async void UpdateCatalogItem_ItemIdEqualsZero_ShouldReturnNotFound()
        {
            // Given
            const string name = "New item";

            var controller = new CatalogController(repository.Object, controllerMapper);
            var item = new CatalogItemUpdateDto() { Id = 0, Name = name };

            // When
            var result = await controller.UpdateCatalogItemAsync(item);

            // Then
            Assert.IsType<NotFoundResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            repository.Verify(r => r.UpdateItem(It.IsAny<int>(), It.IsAny<CatalogItem>()), Times.Never);
            repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async void DeleteCatalogItem_ItemIdEqualsZero_ShouldReturnBadRequest()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var result = await controller.DeleteCatalogItem(0);

            // Then
            Assert.IsType<BadRequestResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Never);
            repository.Verify(r => r.DeleteItem(It.IsAny<int>()), Times.Never);
            repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async void DeleteCatalogItem_ItemIdEqualsTwenty_ShouldReturnNotFound()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var result = await controller.DeleteCatalogItem(20);

            // Then
            Assert.IsType<NotFoundResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            repository.Verify(r => r.DeleteItem(It.IsAny<int>()), Times.Never);
            repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async void DeleteCatalogItem_ItemIdEqualsFifteen_ItemShouldBeDeleted()
        {
            // Given
            var controller = new CatalogController(repository.Object, controllerMapper);

            // When
            var result = await controller.DeleteCatalogItem(15);

            // Then
            Assert.IsType<OkResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            repository.Verify(r => r.DeleteItem(It.IsAny<int>()), Times.Once);
            repository.Verify(r => r.SaveChangesAsync(), Times.Once);

            var deletedItem = await repository.Object.GetItemByIdAsync(15);
            Assert.Null(deletedItem);
        }

        private IMapper CreateControllerMapper()
        {
            return new MapperConfiguration(options => options.AddProfile(new CatalogItemProfile())).CreateMapper();
        }

        private static List<CatalogBrand> GetDefaultCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new() { Id = 1, Brand = "Nike" },
                new() { Id = 2, Brand = "Adidas" },
                new() { Id = 3, Brand = "New Balance" },
                new() { Id = 4, Brand = "Under Armour" },
                new() { Id = 5, Brand = "Reebok" },
                new() { Id = 6, Brand = "FILA" },
                new() { Id = 7, Brand = "Columbia" },
                new() { Id = 8, Brand = "Puma" },
                new() { Id = 9, Brand = "Other" }
            };
        }

        private static List<CatalogType> GetDefaultCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new() { Id = 1, Type = "T-Shirt" },
                new() { Id = 2, Type = "Sneakers" },
                new() { Id = 3, Type = "Shorts" },
                new() { Id = 4, Type = "Sweatpants" },
                new() { Id = 5, Type = "Sweatshirts" },
                new() { Id = 6, Type = "Underwear" },
                new() { Id = 7, Type = "Sport Jacket" },
                new() { Id = 8, Type = "Other" }
            };
        }

        private static List<CatalogItem> GetDefaultCatalogItems()
        {
            return new List<CatalogItem>()
            {
                new()
                {
                    Id = 1, Name = "Adidas Sneakers", Description = "Brand new black & white sneakers",
                    Price = 1200M, PictureFileName = "adidas-sneakers-1.jpg",
                    CatalogTypeId = 2, CatalogBrandId = 2,
                    AvailableInStock = 30, RestockThreshold = 10,
                    MaxStockThreshold = 100, OnReorder = false
                },
                new()
                {
                    Id = 2, Name = "Puma Shorts", Description = "Brand new black shorts",
                    Price = 400M, PictureFileName = "puma-shorts-1.jpg",
                    CatalogTypeId = 3, CatalogBrandId = 8,
                    AvailableInStock = 50, RestockThreshold = 30,
                    MaxStockThreshold = 200, OnReorder = false
                },
                new()
                {
                    Id = 3, Name = "Fila Bag", Description = "Perfect looking FILA sport bag",
                    Price = 800M, PictureFileName = "fila-bag-1.jpg",
                    CatalogTypeId = 8, CatalogBrandId = 6,
                    AvailableInStock = 50, RestockThreshold = 10,
                    MaxStockThreshold = 75, OnReorder = false
                },
                new()
                {
                    Id = 4, Name = "Nike sneakers", Description = "Brand new red sneakers",
                    Price = 1000M, PictureFileName = "nike-sneakers-1.png",
                    CatalogTypeId = 2, CatalogBrandId = 1,
                    AvailableInStock = 50, RestockThreshold = 20,
                    MaxStockThreshold = 100, OnReorder = false
                },
                new()
                {
                    Id = 5, Name = "Nike T-Shirt", Description = "Good-looking white T-Shirt",
                    Price = 900M, PictureFileName = "nike-tshirt-1.jpg",
                    CatalogTypeId = 1, CatalogBrandId = 1,
                    AvailableInStock = 100, RestockThreshold = 50,
                    MaxStockThreshold = 200, OnReorder = false
                },
                new()
                {
                    Id = 6, Name = "Adidas Sweatpants", Description = "Brand new black & white sweatpants",
                    Price = 2200M, PictureFileName = "adidas-sweatpants-1.jpg",
                    CatalogTypeId = 4, CatalogBrandId = 2,
                    AvailableInStock = 10, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                },
                new()
                {
                    Id = 7, Name = "Adidas Sweatshirt", Description = "Brand new black & white sweatshirt",
                    Price = 2000M, PictureFileName = "adidas-sweatshirt-1.jpg",
                    CatalogTypeId = 5, CatalogBrandId = 2,
                    AvailableInStock = 15, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                },
                new()
                {
                    Id = 8, Name = "New Balance Sneakers", Description = "Cool-looking red & blue sneakers",
                    Price = 3000M, PictureFileName = "newbalance-sneakers-1.jpg",
                    CatalogTypeId = 2, CatalogBrandId = 3,
                    AvailableInStock = 15, RestockThreshold = 10,
                    MaxStockThreshold = 40, OnReorder = false
                },
                new()
                {
                    Id = 9, Name = "Under Armour Shorts", Description = "Nice black shorts",
                    Price = 500M, PictureFileName = "underarmour-shorts-1.jpg",
                    CatalogTypeId = 3, CatalogBrandId = 4,
                    AvailableInStock = 50, RestockThreshold = 20,
                    MaxStockThreshold = 100, OnReorder = false
                },
                new()
                {
                    Id = 10, Name = "Nike Underwear", Description = "Black thermal underwear set",
                    Price = 2000M, PictureFileName = "nike-underwear-1.jpg",
                    CatalogTypeId = 6, CatalogBrandId = 1,
                    AvailableInStock = 25, RestockThreshold = 10,
                    MaxStockThreshold = 75, OnReorder = false
                },
                new()
                {
                    Id = 11, Name = "Reebok Sweatshirt", Description = "Blue sweatshirt",
                    Price = 1600M, PictureFileName = "reebok-sweatshirt-1.jpg",
                    CatalogTypeId = 5, CatalogBrandId = 5,
                    AvailableInStock = 15, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                },
                new()
                {
                    Id = 12, Name = "Columbia Jacket", Description = "Winter jacket",
                    Price = 4000M, PictureFileName = "columbia-jacket-1.jpg",
                    CatalogTypeId = 7, CatalogBrandId = 7,
                    AvailableInStock = 10, RestockThreshold = 3,
                    MaxStockThreshold = 30, OnReorder = false
                },
                new()
                {
                    Id = 13, Name = "Fila T-Shirt", Description = "Cool-looking T-Shirt",
                    Price = 600M, PictureFileName = "fila-tshirt-1.jpg",
                    CatalogTypeId = 1, CatalogBrandId = 6,
                    AvailableInStock = 50, RestockThreshold = 15,
                    MaxStockThreshold = 100, OnReorder = false
                },
                new()
                {
                    Id = 14, Name = "Reebok Sweatpants", Description = "Perfect fit pants",
                    Price = 1400M, PictureFileName = "reebok-sweatpants-1.jpg",
                    CatalogTypeId = 4, CatalogBrandId = 5,
                    AvailableInStock = 30, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                },
                new()
                {
                    Id = 15, Name = "Puma Jacket", Description = "Demi-season sports jacket",
                    Price = 2500M, PictureFileName = "puma-jacket-1.jpg",
                    CatalogTypeId = 7, CatalogBrandId = 8,
                    AvailableInStock = 20, RestockThreshold = 5,
                    MaxStockThreshold = 50, OnReorder = false
                }
            };
        }
    }
}