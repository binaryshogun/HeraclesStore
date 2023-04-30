namespace Catalog.FunctionalTests.Services
{
    public static class ApiLinks
    {
        public static string Items() => "api/catalog/items";
        public static string ItemsByType(int typeId) => $"api/catalog/items/type/{typeId}";
        public static string ItemsByBrand(int brandId) => $"api/catalog/items/type/all/brand/{brandId}";
        public static string ItemsByName(string name) => $"api/catalog/items/withname/{name}";
        public static string ItemsByTypeAndBrand(int typeId, int brandId) => $"api/catalog/items/type/{typeId}/brand/{brandId}";

        public static string ItemById(int id) => $"api/catalog/items/{id}";

        public static string Types() => "api/catalog/types";
        public static string Brands() => "api/catalog/brands";
        public static string Create() => Items();
        public static string Update() => Items();
        public static string Delete(int id) => $"api/catalog/items/{id}";
    }
}