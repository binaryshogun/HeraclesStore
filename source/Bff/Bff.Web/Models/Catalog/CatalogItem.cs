namespace Bff.Web.Models.Catalog
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public decimal OldPrice { get; set; }

        public string? PictureUrl { get; set; }
    }
}