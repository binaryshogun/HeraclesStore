namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository repository;

        public CatalogController(ICatalogRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("items")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItem>> GetItemsAsync()
        {
            return await repository.GetItemsAsync();
        }

        [HttpGet("items/type/{catalogTypeId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItem>> GetItemsByTypeAsync(int catalogTypeId)
        {
            return await repository.GetItemsAsync(catalogTypeId: catalogTypeId);
        }

        [HttpGet("items/type/{catalogTypeId:int}/brand/{catalogBrandId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItem>> GetItemsByTypeAndBrandAsync(int catalogTypeId, int catalogBrandId)
        {
            return await repository.GetItemsAsync(catalogBrandId: catalogBrandId, catalogTypeId: catalogTypeId);
        }

        [HttpGet("items/type/all/brand/{catalogBrandId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItem>> GetItemsByBrandAsync(int catalogBrandId)
        {
            return await repository.GetItemsAsync(catalogBrandId: catalogBrandId);
        }

        [HttpGet("items/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItem>> GetItemsByNameAsync(string name)
        {
            return await repository.GetItemsAsync(name: name);
        }

        [HttpGet("items/{itemId:int}")]
        [ProducesResponseType(typeof(CatalogItem), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatalogItem>> GetItemByIdAsync(int itemId)
        {
            var item = await repository.GetItemByIdAsync(itemId);

            if (item is null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpGet("types")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogType>> GetTypes()
        {
            return await repository.GetAllTypesAsync();
        }

        [HttpGet("brands")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogBrand>> GetBrandsAsync()
        {
            return await repository.GetAllBrandsAsync();
        }
    }
}