namespace Basket.Api.Data
{
    public interface IBasketRepository
    {
        public IEnumerable<string> GetCustomers();

        Task<CustomerBasket?> GetBasketAsync(string customerId);
        Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string customerId);
    }
}