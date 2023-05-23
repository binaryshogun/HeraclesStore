namespace Bff.Web.Services.Catalog
{
    public interface ICatalogService
    {
        Task<CatalogItem> GetCatalogItemAsync(int id);
        Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids);
    }
}