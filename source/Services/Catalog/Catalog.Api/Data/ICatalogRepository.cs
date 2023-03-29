namespace Catalog.Api.Data
{
    /// <summary>
    /// Repository for <see cref="CatalogContext" />.
    /// </summary>
    public interface ICatalogRepository
    {
        Task<IEnumerable<CatalogItem>> GetAllItemsAsync();
        Task<IEnumerable<CatalogItem>> GetItemsByBrandAsync(int catalogBrandId);
        Task<IEnumerable<CatalogItem>> GetItemsByTypeAsync(int catalogTypeId);
        Task<IEnumerable<CatalogItem>> GetItemsByTypeAndBrandAsync(int catalogTypeId, int catalogBrandId);

        Task<CatalogItem?> GetItemByIdAsync(int itemId);
        CatalogItem CreateItem(CatalogItem item);
        bool UpdateItem(int itemId, CatalogItem item);
        void DeleteItem(int itemId);

        Task<IEnumerable<CatalogBrand>> GetAllBrandsAsync();
        Task<IEnumerable<CatalogType>> GetAllTypesAsync();
    }
}