using System.Text.Json;

namespace Bff.Web.Services.Ordering
{
    public class OrderingApiClient : IOrderingApiClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<OrderingApiClient> logger;

        public OrderingApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<OrderingApiClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<OrderDraftDto?> GetOrderDraftFromBasketAsync(CustomerBasketDto basket)
        {
            var url = $"{configuration.GetSection("Urls").GetValue<string>("Ordering")}/draft";
            var content = new StringContent(JsonSerializer.Serialize(basket), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            var ordersDraftResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<OrderDraftDto>(ordersDraftResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}