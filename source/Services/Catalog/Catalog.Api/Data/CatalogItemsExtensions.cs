namespace Catalog.Api.Data
{
    public static class CatalogItemsExtensions
    {
        public static IQueryable<CatalogItem> FilterByBrand(this IQueryable<CatalogItem> items, int? catalogBrandId)
        {
            return catalogBrandId is null ? items : items.Where(i => i.CatalogBrandId == catalogBrandId);
        }

        public static IQueryable<CatalogItem> FilterByType(this IQueryable<CatalogItem> items, int? catalogTypeId)
        {
            return catalogTypeId is null ? items : items.Where(i => i.CatalogTypeId == catalogTypeId);
        }

        public static IQueryable<CatalogItem> FilterByName(this IQueryable<CatalogItem> items, string? name)
        {
            return name is null ? items : items.Where(i => i.Name != null && i.Name.StartsWith(name));
        }
    }
}