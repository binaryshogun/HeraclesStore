namespace Ordering.Api.Application.Models
{
    public class CustomerBasket
    {
        public CustomerBasket(Guid customerId, List<BasketItem> items)
        {
            CustomerId = customerId != Guid.Empty ? customerId : throw new ArgumentException(nameof(customerId));
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public Guid CustomerId { get; init; }
        public List<BasketItem> Items { get; init; }
    }
}