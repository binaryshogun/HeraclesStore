namespace Catalog.UnitTests.Mocks
{
    public class CatalogDataMocks
    {
        public static Mock<ICatalogRepository> CreateRepositoryMock(
            List<CatalogItem> items,
            List<CatalogBrand> brands,
            List<CatalogType> types)
        {
            var repository = new Mock<ICatalogRepository>();

            repository
                .Setup(r => r.GetItemsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<string?>()))
                .Returns((int? catalogBrandId, int? catalogTypeId, string name) =>
                    Task.FromResult(items
                        .AsQueryable()
                        .FilterByBrand(catalogBrandId)
                        .FilterByType(catalogTypeId)
                        .FilterByName(name)
                        .AsEnumerable()));

            repository.Setup(r => r.GetAllBrandsAsync()).Returns(Task.FromResult(brands.AsEnumerable()));
            repository.Setup(r => r.GetAllTypesAsync()).Returns(Task.FromResult(types.AsEnumerable()));

            repository
                .Setup(r => r.GetItemByIdAsync(It.IsAny<int>()))
                .Returns((int itemId) => Task.FromResult(items.FirstOrDefault(i => i.Id == itemId)));

            repository
                .Setup(r => r.CreateItem(It.IsAny<CatalogItem>()))
                .Returns((CatalogItem item) =>
                {
                    item.Id = items.Max(i => i.Id) + 1;
                    items.Add(item);
                    return item;
                });

            repository
                .Setup(r => r.UpdateItem(It.IsAny<int>(), It.IsAny<CatalogItem>()))
                .Returns((int itemId, CatalogItem item) =>
                {
                    if (itemId != item.Id)
                    {
                        return false;
                    }

                    var itemToReplace = items.FirstOrDefault(i => i.Id == itemId);

                    if (itemToReplace is null)
                    {
                        return false;
                    }

                    var index = items.IndexOf(itemToReplace);
                    items[index] = item;

                    return true;
                });

            repository
                .Setup(r => r.DeleteItem(It.IsAny<int>()))
                .Callback((int itemId) =>
                {
                    var item = items.FirstOrDefault(i => i.Id == itemId);
                    if (item is not null)
                    {
                        items.Remove(item);
                    }
                });

            repository.Setup(r => r.SaveChanges()).Returns(true);
            repository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            return repository;
        }
    }
}