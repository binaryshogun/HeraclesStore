namespace Catalog.Api.Data.Dtos
{
    public class CatalogItemCreateDto
    {
        [Required]
        [MinLength(10)]
        [MaxLength(50)]
        public string? Name { get; init; }

        [Required]
        [MinLength(30)]
        [MaxLength(200)]
        public string? Description { get; init; }

        [Required]
        [Precision(8, 2)]
        [Range(1, 1000000)]
        public decimal Price { get; init; }
        [Required]
        [Range(0, 99)]
        [Precision(2, 0)]
        public decimal Discount { get; set; }
        public string? PictureFileName { get; init; }

        [Required]
        public int CatalogTypeId { get; init; }
        [Required]
        public int CatalogBrandId { get; init; }

        [Required]
        public int AvailableInStock { get; init; }
        [Required]
        public int RestockThreshold { get; init; }
        [Required]
        public int MaxStockThreshold { get; init; }
        [Required]
        public bool OnReorder { get; init; }
    }
}