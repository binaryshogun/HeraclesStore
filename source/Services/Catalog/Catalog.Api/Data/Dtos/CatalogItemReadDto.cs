namespace Catalog.Api.Data.Dtos
{
    public class CatalogItemReadDto
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public string? PictureFileName { get; init; }

        public int CatalogTypeId { get; init; }
        public int CatalogBrandId { get; init; }

        public int AvailableInStock { get; init; }
    }
}