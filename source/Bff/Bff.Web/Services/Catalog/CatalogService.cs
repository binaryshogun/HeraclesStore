namespace Bff.Web.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly GrpcCatalog.GrpcCatalogClient client;
        private readonly ILogger<CatalogService> logger;

        public CatalogService(GrpcCatalog.GrpcCatalogClient client, ILogger<CatalogService> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            var request = new CatalogItemRequest { Id = id };
            logger.LogInformation("grpc request {@request}", request);
            var response = await client.GetItemByIdAsync(request);
            logger.LogInformation("grpc response {@response}", response);
            return MapToCatalogItemResponse(response);

        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            var request = new CatalogItemsRequest();
            foreach (int itemId in ids)
            {
                request.Ids.Add(itemId);
            }

            logger.LogInformation("grpc request {@request}", request);
            var response = await client.GetItemsByIdsAsync(request);
            logger.LogInformation("grpc response {@response}", response);
            return response.Items.Select(this.MapToCatalogItemResponse);
        }

        private CatalogItem MapToCatalogItemResponse(CatalogItemResponse catalogItemResponse)
        {
            return new CatalogItem
            {
                Id = catalogItemResponse.Id,
                Name = catalogItemResponse.Name,
                PictureUrl = catalogItemResponse.PictureUri,
                Price = (decimal)catalogItemResponse.Price
            };
        }
    }
}