namespace Catalog.UnitTests.Models
{
    public class CatalogItemTests
    {
        [Fact]
        public void RemoveStock_ZeroQuantity_ShouldThrowCatalogDomainException()
        {
            // Arrange
            var item = new CatalogItem()
            {
                AvailableInStock = 1
            };

            var action = new Action(() => item.RemoveStock(0));

            // Act & Assert
            Assert.Throws<CatalogDomainException>(action);
        }

        [Fact]
        public void RemoveStock_ZeroStockAvailable_ShouldThrowCatalogDomainException()
        {
            // Arrange
            var item = new CatalogItem();
            var action = new Action(() => item.RemoveStock(0));

            // Act & Assert
            Assert.Throws<CatalogDomainException>(action);
        }

        [Fact]
        public void RemoveStock_FiveItems_FiveAvailable_ShouldReturnFive()
        {
            // Arrange
            var item = new CatalogItem()
            {
                AvailableInStock = 5
            };

            // Act
            var result = item.RemoveStock(5);

            // Assert
            Assert.Equal(5, result);
            Assert.Equal(0, item.AvailableInStock);
            Assert.True(item.OnReorder);
        }

        [Fact]
        public void RemoveStock_FiveItems_TenAvailable_ShouldReturnFive()
        {
            // Arrange
            var item = new CatalogItem()
            {
                AvailableInStock = 10
            };

            // Act
            var result = item.RemoveStock(5);

            // Assert
            Assert.Equal(5, result);
            Assert.Equal(5, item.AvailableInStock);
            Assert.False(item.OnReorder);
        }

        [Fact]
        public void RemoveStock_FiveItems_ThreeAvailable_ShouldReturnThree()
        {
            // Arrange
            var item = new CatalogItem()
            {
                AvailableInStock = 3
            };

            // Act
            var result = item.RemoveStock(5);

            // Assert
            Assert.Equal(3, result);
            Assert.Equal(0, item.AvailableInStock);
            Assert.True(item.OnReorder);
        }

        [Fact]
        public void AddStock_ZeroQuantity_ShouldThrowCatalogDomainException()
        {
            // Arrange
            var item = new CatalogItem();
            var action = new Action(() => item.AddStock(0));

            // Act & Assert
            Assert.Throws<CatalogDomainException>(action);
        }

        [Fact]
        public void AddStock_FiveItems_MaxStockThresholdUnlimited_ShouldReturnFive()
        {
            // Arrange
            var item = new CatalogItem()
            {
                MaxStockThreshold = int.MaxValue
            };

            // Act
            var result = item.AddStock(5);

            // Assert
            Assert.Equal(5, result);
            Assert.Equal(5, item.AvailableInStock);
            Assert.False(item.OnReorder);
        }

        [Fact]
        public void AddStock_FiveItems_MaxStockThresholdLimitedAsThree_ShouldReturnThree()
        {
            // Arrange
            var item = new CatalogItem()
            {
                MaxStockThreshold = 3
            };

            // Act
            var result = item.AddStock(5);

            // Assert
            Assert.Equal(3, result);
            Assert.Equal(3, item.AvailableInStock);
            Assert.False(item.OnReorder);
        }

        [Fact]
        public void AddStock_FiveItems_RestockThresholdIsThree_OneAvailable_ShouldReturnThree()
        {
            // Arrange
            var item = new CatalogItem()
            {
                AvailableInStock = 1,
                RestockThreshold = 3,
                MaxStockThreshold = 5,
                OnReorder = true
            };

            // Act
            var result = item.AddStock(3);

            // Assert
            Assert.Equal(3, result);
            Assert.Equal(4, item.AvailableInStock);
            Assert.False(item.OnReorder);
        }

        [Fact]
        public void FillPicturePath_PictureUriShouldReturnImageUrl()
        {
            // Arrange
            const string fileName = "photo.jpg";
            const string basePath = "http://localhost:5000/";

            /* http://localhost:5000/photo.jpg */
            var expectedPath = basePath + fileName;

            var item = new CatalogItem()
            {
                PictureFileName = fileName
            };

            // Act
            item.FillPicturePath(basePath);

            // Assert
            Assert.Equal(expectedPath, item.PictureUri);
        }
    }
}