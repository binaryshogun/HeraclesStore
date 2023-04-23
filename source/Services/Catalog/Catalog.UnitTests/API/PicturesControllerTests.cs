using Microsoft.AspNetCore.Http;

namespace Catalog.UnitTests.API
{
    public class PicturesControllerTests
    {
        private const string UploadedImagePath = "test-picture-1.jpg";
        private Mock<IWebHostEnvironment> environment;
        private Mock<ICatalogRepository> repository;

        public PicturesControllerTests()
        {
            environment = SetupEnvironment();
            repository = SetupRepository();
        }

        [Fact]
        public async Task GetImage_ItemIdGreaterThanOne_ShouldReturnFileContent()
        {
            // Given
            var controller = new PicturesController(environment.Object, repository.Object);

            // When
            var result = await controller.GetImageAsync(2);

            // Then
            Assert.IsType<FileContentResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            environment.Verify(e => e.WebRootPath, Times.Once);
        }

        [Fact]
        public async Task GetImage_ItemIdEqualsOne_ShouldReturnNotFound()
        {
            // Given
            var controller = new PicturesController(environment.Object, repository.Object);

            // When
            var result = await controller.GetImageAsync(1);

            // Then
            Assert.IsType<NotFoundResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            environment.Verify(e => e.WebRootPath, Times.Never);
        }

        [Fact]
        public async Task GetImage_ItemIdEqualsZero_ShouldReturnBadRequest()
        {
            // Given
            var controller = new PicturesController(environment.Object, repository.Object);

            // When
            var result = await controller.GetImageAsync(0);

            // Then
            Assert.IsType<BadRequestResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Never);
            environment.Verify(e => e.WebRootPath, Times.Never);
        }

        [Fact]
        public async Task CreateImage_ItemIdEqualsOne_ImageFile_ShouldUploadImageAndReturnOk()
        {
            // Given
            var controller = new PicturesController(environment.Object, repository.Object);
            var file = CreateFormFileMock();

            // When
            var result = await controller.CreateImageAsync(1, file.Object);

            // Then
            Assert.IsType<OkResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            environment.Verify(e => e.WebRootPath, Times.Once);
            file.Verify(f => f.FileName, Times.Exactly(2));
            file.Verify(f => f.Length, Times.Once);
            file.Verify(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);

            var updatedItem = await repository.Object.GetItemByIdAsync(1);
            Assert.NotNull(updatedItem);
            Assert.NotNull(updatedItem.PictureFileName);
            Assert.Equal(UploadedImagePath, updatedItem.PictureFileName);

            File.Delete(Path.Combine(environment.Object.WebRootPath, "test-picture-1.jpg"));
        }

        [Fact]
        public async Task CreateImage_ItemIdEqualsOne_ZeroFileLength_ShouldReturnBadRequest()
        {
            // Given
            var controller = new PicturesController(environment.Object, repository.Object);
            var file = CreateFormFileMock();
            file.Setup(f => f.Length).Returns(0);

            // When
            var result = await controller.CreateImageAsync(1, file.Object);

            // Then
            Assert.IsType<BadRequestResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            environment.Verify(e => e.WebRootPath, Times.Never);
            file.Verify(f => f.FileName, Times.Once);
            file.Verify(f => f.Length, Times.Once);
            file.Verify(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);

            var item = await repository.Object.GetItemByIdAsync(1);
            Assert.NotNull(item);
            Assert.Null(item.PictureFileName);
        }

        [Fact]
        public async Task CreateImage_ItemIdEqualsOne_NotImageFile_ShouldReturnBadRequest()
        {
            // Given
            var controller = new PicturesController(environment.Object, repository.Object);
            var file = CreateFormFileMock();
            file.Setup(f => f.FileName).Returns("abcd");

            // When
            var result = await controller.CreateImageAsync(1, file.Object);

            // Then
            Assert.IsType<BadRequestResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            environment.Verify(e => e.WebRootPath, Times.Never);
            file.Verify(f => f.FileName, Times.Once);
            file.Verify(f => f.Length, Times.Once);
            file.Verify(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);

            var item = await repository.Object.GetItemByIdAsync(1);
            Assert.NotNull(item);
            Assert.Null(item.PictureFileName);
        }

        [Fact]
        public async Task CreateImage_ItemIdEqualsZero_ImageFile_ShouldReturnNotFound()
        {
            // Given
            var controller = new PicturesController(environment.Object, repository.Object);
            var file = CreateFormFileMock();

            // When
            var result = await controller.CreateImageAsync(0, file.Object);

            // Then
            Assert.IsType<NotFoundResult>(result);
            repository.Verify(r => r.GetItemByIdAsync(It.IsAny<int>()), Times.Once);
            environment.Verify(e => e.WebRootPath, Times.Never);
            file.Verify(f => f.FileName, Times.Never);
            file.Verify(f => f.Length, Times.Never);
            file.Verify(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private Mock<IFormFile> CreateFormFileMock()
        {
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.FileName).Returns(UploadedImagePath);
            fileMock.Setup(f => f.Length).Returns(1);

            fileMock
                .Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) =>
                {
                    return Task.CompletedTask;
                });

            return fileMock;
        }

        private Mock<IWebHostEnvironment> SetupEnvironment()
        {
            var environment = new Mock<IWebHostEnvironment>();

            var netDirectoryPath = Directory.GetCurrentDirectory();
            var projectDirectory = Directory.GetParent(netDirectoryPath)?.Parent?.Parent?.FullName;

            environment.Setup(e => e.WebRootPath).Returns($"{projectDirectory}/Pictures");

            return environment;
        }

        private Mock<ICatalogRepository> SetupRepository()
        {
            var repository = new Mock<ICatalogRepository>();

            var defaultItem = new CatalogItem()
            {
                PictureFileName = "test-picture.jpg"
            };

            var emptyItem = new CatalogItem()
            {
                PictureFileName = null
            };

            repository
                .Setup(r => r.GetItemByIdAsync(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(id <= 0 ? null : id == 1 ? emptyItem : defaultItem));

            repository
                .Setup(r => r.UpdateItem(It.IsAny<int>(), It.IsAny<CatalogItem>()))
                .Returns((int id, CatalogItem item) =>
                {
                    defaultItem.PictureFileName = item.PictureFileName;
                    return true;
                });

            repository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            return repository;
        }
    }
}