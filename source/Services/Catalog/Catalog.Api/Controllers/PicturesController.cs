namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/catalog/items")]
    public class PicturesController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly ICatalogRepository repository;

        public PicturesController(IWebHostEnvironment environment, ICatalogRepository repository)
        {
            this.environment = environment;
            this.repository = repository;
        }

        [HttpGet("{catalogItemId:int}/picture")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK, "image/jpeg", "image/png")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetImageAsync(int catalogItemId)
        {
            if (catalogItemId <= 0)
            {
                return BadRequest();
            }

            var item = await repository.GetItemByIdAsync(catalogItemId);

            if (item is null || item.PictureFileName is null)
            {
                return NotFound();
            }

            var webRoot = environment.WebRootPath;
            var path = Path.Combine(webRoot, item.PictureFileName);

            string? extension = Path.GetExtension(item.PictureFileName);
            string mimetype = GetImageMimeTypeFromImageExtension(extension);

            var content = await System.IO.File.ReadAllBytesAsync(path);

            return File(content, mimetype);
        }

        [HttpPost("{catalogItemId:int}/picture")]
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest, "application/json")]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound, "application/json")]
        public async Task<ActionResult> CreateImageAsync(int catalogItemId, IFormFile file)
        {
            var item = await repository.GetItemByIdAsync(catalogItemId);

            if (item is null)
            {
                return NotFound();
            }

            var extension = Path.GetExtension(file.FileName);

            if (file.Length == 0 || !IsImage(extension))
            {
                return BadRequest();
            }

            string fileName = file.FileName;
            string webRoot = environment.WebRootPath;

            string path = Path.Combine(webRoot, fileName);

            if (Path.Exists(path))
            {
                fileName = $"new-{file.FileName}";
                path = Path.Combine(webRoot, fileName);
            }

            using (Stream fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            item.PictureFileName = fileName;

            repository.UpdateItem(item.Id, item);
            await repository.SaveChangesAsync();

            return Ok();
        }

        private bool IsImage(string? extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" => true,
                _ => false
            };
        }

        /// <summary>
        /// Gets content type for the given image <paramref name="extension" />.
        /// </summary>
        /// <param name="extension">Extension of the image.</param>
        /// <returns>Mime content type for the given image <paramref name="extension" />.</returns>
        private string GetImageMimeTypeFromImageExtension(string? extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
}