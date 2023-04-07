namespace Basket.Api.Models
{
    public class CustomerBasket
    {
        public CustomerBasket() { }

        public CustomerBasket(string customerId)
        {
            CustomerId = customerId;
        }

        [Required]
        public string CustomerId { get; init; } = default!;
        public List<BasketItem> Items { get; init; } = new();
    }
}