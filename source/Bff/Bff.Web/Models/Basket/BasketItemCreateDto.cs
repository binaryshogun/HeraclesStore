namespace Bff.Web.Models.Basket
{
    public class BasketItemCreateDto
    {
        public Guid Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}