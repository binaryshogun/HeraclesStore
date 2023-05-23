namespace Bff.Web.Models.Basket
{
    public class UpdateBasketItemsRequest
    {
        public UpdateBasketItemsRequest()
        {
            UpdatedItems = new List<BasketItemUpdateDto>();
        }

        public string? BuyerId { get; set; }
        public ICollection<BasketItemUpdateDto> UpdatedItems { get; set; }
    }
}