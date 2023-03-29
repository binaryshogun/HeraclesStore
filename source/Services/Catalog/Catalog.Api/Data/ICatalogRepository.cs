namespace Catalog.Api.Data
{
    /// <summary>
    /// Repository for <see cref="CatalogContext" />.
    /// </summary>
    public interface ICatalogRepository
    {
        Task<IEnumerable<CatalogItem>> GetItemsAsync(int? catalogBrandId = null, int? catalogTypeId = null, string? name = null);
        Task<IEnumerable<CatalogBrand>> GetAllBrandsAsync();
        Task<IEnumerable<CatalogType>> GetAllTypesAsync();

        Task<CatalogItem?> GetItemByIdAsync(int itemId);
        CatalogItem CreateItem(CatalogItem item);
        bool UpdateItem(int itemId, CatalogItem item);
        void DeleteItem(int itemId);
    }
}