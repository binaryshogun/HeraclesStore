namespace Basket.Api.Data
{
    public class RedisBasketRepository : IBasketRepository
    {
        private readonly ConnectionMultiplexer redis;
        private IDatabase database;

        private readonly ILogger<RedisBasketRepository> logger;

        public RedisBasketRepository(ConnectionMultiplexer redis, ILogger<RedisBasketRepository> logger)
        {
            this.redis = redis;
            database = redis.GetDatabase();

            this.logger = logger;
        }

        public IEnumerable<string> GetCustomers()
        {
            var server = GetServer();
            var data = server.Keys();

            return data?.Select(k => k.ToString()) ?? new List<string>();
        }

        public async Task<CustomerBasket?> GetBasketAsync(string customerId)
        {
            var data = await database.StringGetAsync(customerId);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<CustomerBasket>(data!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var created = await database.StringSetAsync(basket.CustomerId, JsonSerializer.Serialize(basket));

            if (!created)
            {
                logger.LogInformation("[Basket] ---> Problem occurred while persisting the item");
                return null;
            }

            logger.LogInformation("[Basket] ---> Basket item persisted successfully");

            return await GetBasketAsync(basket.CustomerId);
        }

        public async Task<bool> DeleteBasketAsync(string customerId)
        {
            return await database.KeyDeleteAsync(customerId);
        }

        private IServer GetServer()
        {
            var endpoint = redis.GetEndPoints();
            return redis.GetServer(endpoint.FirstOrDefault());
        }
    }
}