namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository repository;
        private readonly IMapper mapper;

        public CatalogController(ICatalogRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet("items")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItemReadDto>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItemReadDto>> GetItemsAsync()
        {
            return mapper.Map<IEnumerable<CatalogItemReadDto>>(await repository.GetItemsAsync());
        }

        [HttpGet("items/type/{catalogTypeId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItemReadDto>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItemReadDto>> GetItemsByTypeAsync(int catalogTypeId)
        {
            return mapper.Map<IEnumerable<CatalogItemReadDto>>(await repository.GetItemsAsync(catalogTypeId: catalogTypeId));
        }

        [HttpGet("items/type/{catalogTypeId:int}/brand/{catalogBrandId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItemReadDto>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItemReadDto>> GetItemsByTypeAndBrandAsync(int catalogTypeId, int catalogBrandId)
        {
            return mapper.Map<IEnumerable<CatalogItemReadDto>>(await repository.GetItemsAsync(catalogBrandId: catalogBrandId, catalogTypeId: catalogTypeId));
        }

        [HttpGet("items/type/all/brand/{catalogBrandId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItemReadDto>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItemReadDto>> GetItemsByBrandAsync(int catalogBrandId)
        {
            return mapper.Map<IEnumerable<CatalogItemReadDto>>(await repository.GetItemsAsync(catalogBrandId: catalogBrandId));
        }

        [HttpGet("items/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(IEnumerable<CatalogItemReadDto>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogItemReadDto>> GetItemsByNameAsync(string name)
        {
            return mapper.Map<IEnumerable<CatalogItemReadDto>>(await repository.GetItemsAsync(name: name));
        }

        [HttpGet("items/{catalogItemId:int}")]
        [ActionName(nameof(GetItemByIdAsync))]
        [ProducesResponseType(typeof(CatalogItemReadDto), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatalogItemReadDto>> GetItemByIdAsync(int catalogItemId)
        {
            if (catalogItemId <= 0)
            {
                return BadRequest();
            }

            var item = await repository.GetItemByIdAsync(catalogItemId);

            if (item is null)
            {
                return NotFound();
            }

            return mapper.Map<CatalogItemReadDto>(item);
        }

        [HttpGet("types")]
        [ProducesResponseType(typeof(IEnumerable<CatalogType>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogType>> GetTypes()
        {
            return await repository.GetAllTypesAsync();
        }

        [HttpGet("brands")]
        [ProducesResponseType(typeof(IEnumerable<CatalogBrand>), StatusCodes.Status200OK, "application/json")]
        public async Task<IEnumerable<CatalogBrand>> GetBrandsAsync()
        {
            return await repository.GetAllBrandsAsync();
        }

        [HttpPost("items")]
        [ProducesResponseType(typeof(CatalogItemReadDto), StatusCodes.Status201Created, "application/json")]
        public ActionResult CreateCatalogItem(CatalogItemCreateDto catalogItemCreateDto)
        {
            var item = mapper.Map<CatalogItem>(catalogItemCreateDto);

            var createdItem = repository.CreateItem(item);
            repository.SaveChanges();

            var catalogItemReadDto = mapper.Map<CatalogItemReadDto>(createdItem);

            return CreatedAtAction(nameof(GetItemByIdAsync), new { Id = catalogItemReadDto.Id }, catalogItemReadDto);
        }

        [HttpPut("items")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateCatalogItemAsync(CatalogItemUpdateDto catalogItemUpdateDto)
        {
            var item = await repository.GetItemByIdAsync(catalogItemUpdateDto.Id);

            if (item is null)
            {
                return NotFound();
            }

            item = mapper.Map<CatalogItem>(catalogItemUpdateDto);

            try
            {
                repository.UpdateItem(item.Id, item);
                await repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(ex);
            }

            return NoContent();
        }

        [HttpDelete("items/{catalogItemId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteCatalogItem(int catalogItemId)
        {
            if (catalogItemId <= 0)
            {
                return BadRequest();
            }

            var item = await repository.GetItemByIdAsync(catalogItemId);

            if (item is null)
            {
                return NotFound();
            }

            try
            {
                repository.DeleteItem(catalogItemId);
                await repository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(ex);
            }

            return Ok();
        }
    }
}