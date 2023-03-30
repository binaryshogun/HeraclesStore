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

        public async Task<IEnumerable<CatalogItem>> GetItemsAsync(int? catalogBrandId = null, int? catalogTypeId = null, string? name = null)
        {
            return await context.CatalogItems
                .FilterByBrand(catalogBrandId)
                .FilterByType(catalogTypeId)
                .FilterByName(name)
                .ToListAsync();
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

        public bool SaveChanges()
        {
            return context.SaveChanges() > 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}