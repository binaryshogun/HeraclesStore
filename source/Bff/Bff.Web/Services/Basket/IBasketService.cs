namespace Bff.Web.Services.Basket
{
    public interface IBasketService
    {
        Task<CustomerBasketDto?> GetByIdAsync(string? buyerId);
        Task UpdateAsync(CustomerBasketDto currentBasket);
    }
}