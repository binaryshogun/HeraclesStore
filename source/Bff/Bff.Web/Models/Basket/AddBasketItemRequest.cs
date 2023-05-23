namespace Bff.Web.Models.Basket
{
    public class AddBasketItemRequest
    {
        public AddBasketItemRequest()
        {
            Quantity = 1;
        }

        public int Quantity { get; init; }
        public string? BasketId { get; init; }
        public int CatalogItemId { get; init; }
    }
}