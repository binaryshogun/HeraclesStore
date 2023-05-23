namespace Bff.Web.Models.Basket
{
    public class UpdateBasketRequest
    {
        public string? BuyerId { get; set; }

        public IEnumerable<BasketItemCreateDto>? Items { get; set; }
    }
}