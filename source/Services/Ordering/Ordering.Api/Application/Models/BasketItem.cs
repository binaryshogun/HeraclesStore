namespace Ordering.Api.Application.Models
{
    public class BasketItem
    {
        public Guid Id { get; init; }

        public int ProductId { get; init; }

        [Required]
        public string? ProductName { get; init; }

        public decimal UnitPrice { get; init; }

        public decimal OldUnitPrice { get; init; }

        [Range(1, 1000)]
        public int Quantity { get; set; }

        public string? PictureUrl { get; init; }
    }
}