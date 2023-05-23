namespace Bff.Web.Models.Basket
{
    public class CustomerBasketDto
    {
        public CustomerBasketDto() { }

        public CustomerBasketDto(string? buyerId)
        {
            BuyerId = buyerId;
        }

        public string? BuyerId { get; set; }
        public List<BasketItemReadDto> Items { get; set; } = new();
    }
}