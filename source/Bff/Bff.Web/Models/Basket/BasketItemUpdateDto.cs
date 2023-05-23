namespace Bff.Web.Models.Basket
{
    public class BasketItemUpdateDto
    {
        public Guid BasketItemId { get; set; }
        public int Quantity { get; set; }
    }
}