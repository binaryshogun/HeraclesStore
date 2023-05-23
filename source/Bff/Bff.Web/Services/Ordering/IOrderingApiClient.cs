namespace Bff.Web.Services.Ordering
{
    public interface IOrderingApiClient
    {
        Task<OrderDraftDto?> GetOrderDraftFromBasketAsync(CustomerBasketDto basket);
    }
}