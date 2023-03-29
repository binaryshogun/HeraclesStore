namespace Catalog.Api.Data
{
    /// <summary>
    /// Class that implements <see cref="ICatalogRepository" /> using EntityFramework.
    /// </summary>
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CatalogContext context;

        public CatalogRepository(CatalogContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<CatalogItem>> GetAllItemsAsync()
        {
            return await context.CatalogItems.ToListAsync();
        }

        public async Task<CatalogItem?> GetItemByIdAsync(int itemId)
        {
            return await context.CatalogItems.FindAsync(itemId);
        }

        public CatalogItem CreateItem(CatalogItem item)
        {
            return context.CatalogItems.Add(item).Entity;
        }

        public bool UpdateItem(int itemId, CatalogItem item)
        {
            if (itemId == item.Id)
            {
                context.Entry(item).State = EntityState.Modified;
                return true;
            }

            return false;
        }

        public void DeleteItem(int itemId)
        {
            var item = context.CatalogItems.Find(itemId);

            if (item is not null)
            {
                context.CatalogItems.Remove(item);
            }
        }

        public async Task<IEnumerable<CatalogBrand>> GetAllBrandsAsync()
        {
            return await context.CatalogBrands.ToListAsync();
        }

        public async Task<IEnumerable<CatalogType>> GetAllTypesAsync()
        {
            return await context.CatalogTypes.ToListAsync();
        }

        public async Task<IEnumerable<CatalogItem>> GetItemsByBrandAsync(int catalogBrandId)
        {
            return await context.CatalogItems
                .Where(ci => ci.CatalogBrandId == catalogBrandId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CatalogItem>> GetItemsByTypeAsync(int catalogTypeId)
        {
            return await context.CatalogItems
                .Where(ci => ci.CatalogTypeId == catalogTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CatalogItem>> GetItemsByTypeAndBrandAsync(int catalogTypeId, int catalogBrandId)
        {
            return await context.CatalogItems
                .Where(ci => ci.CatalogBrandId == catalogBrandId && ci.CatalogTypeId == catalogTypeId)
                .ToListAsync();
        }
    }
}