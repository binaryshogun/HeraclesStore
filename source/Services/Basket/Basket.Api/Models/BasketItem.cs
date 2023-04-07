namespace Basket.Api.Models
{
    public class BasketItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        public string? ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal OldUnitPrice { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }

        public string? PictureUrl { get; set; }
    }
}