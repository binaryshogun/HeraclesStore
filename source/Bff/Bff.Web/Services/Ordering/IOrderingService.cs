namespace Bff.Web.Services.Ordering
{
    public interface IOrderingService
    {
        Task<OrderDraftDto?> GetOrderDraftAsync(CustomerBasketDto basketData);
    }
}