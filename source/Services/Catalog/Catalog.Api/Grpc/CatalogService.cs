namespace Catalog.Api.Grpc
{
    public class CatalogService : GrpcCatalog.GrpcCatalogBase
    {
        private readonly CatalogContext catalogContext;
        private readonly string? pictureBaseUrl;
        private readonly ILogger<CatalogService> logger;

        public CatalogService(CatalogContext catalogContext, IConfiguration configuration, ILogger<CatalogService> logger)
        {
            this.catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            this.pictureBaseUrl = configuration.GetValue<string>("PictureBaseUrl");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CatalogItemResponse?> GetItemById(CatalogItemRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call CatalogService.GetItemById for product id {Id}", request.Id);
            if (request.Id <= 0)
            {
                context.Status = new Status(StatusCode.FailedPrecondition, $"Id must be > 0 (received {request.Id})");
                return null;
            }

            var item = await catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == request.Id);

            if (item is not null)
            {
                item.FillPicturePath(pictureBaseUrl!);

                return new CatalogItemResponse()
                {
                    AvailableInStock = item.AvailableInStock,
                    Description = item.Description,
                    Id = item.Id,
                    MaxStockThreshold = item.MaxStockThreshold,
                    Name = item.Name,
                    OnReorder = item.OnReorder,
                    PictureFileName = item.PictureFileName,
                    PictureUri = item.PictureUri,
                    Price = (double)item.Price,
                    RestockThreshold = item.RestockThreshold
                };
            }

            context.Status = new Status(StatusCode.NotFound, $"Product with given id ({request.Id}) do not exist");
            return null;
        }

        public override async Task<CatalogItemsResponse> GetItemsByIds(CatalogItemsRequest request, ServerCallContext context)
        {
            if (request.Ids is not null and { Count: > 0 })
            {
                var items = await GetItemsByIdsAsync(request.Ids.ToList());

                context.Status = !items.Any() ?
                    new Status(StatusCode.NotFound, $"ids value invalid. Must be list of numbers") :
                    new Status(StatusCode.OK, string.Empty);

                return MapToResponse(items);
            }

            var itemsOnPage = await catalogContext.CatalogItems.OrderBy(c => c.Name).ToListAsync();
            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = MapToResponse(itemsOnPage);
            context.Status = new Status(StatusCode.OK, string.Empty);

            return model;
        }

        private CatalogItemsResponse MapToResponse(List<CatalogItem> items)
        {
            var result = new CatalogItemsResponse();

            items.ForEach(i =>
            {
                var brand = i.CatalogBrand == null ? null : new CatalogItemBrand()
                {
                    Id = i.CatalogBrand.Id,
                    Brand = i.CatalogBrand.Brand,
                };
                var catalogType = i.CatalogType == null ? null : new CatalogItemType()
                {
                    Id = i.CatalogType.Id,
                    Type = i.CatalogType.Type,
                };

                result.Items.Add(new CatalogItemResponse()
                {
                    AvailableInStock = i.AvailableInStock,
                    Description = i.Description,
                    Id = i.Id,
                    MaxStockThreshold = i.MaxStockThreshold,
                    Name = i.Name,
                    OnReorder = i.OnReorder,
                    PictureFileName = i.PictureFileName,
                    PictureUri = i.PictureUri,
                    RestockThreshold = i.RestockThreshold,
                    CatalogBrand = brand,
                    CatalogType = catalogType,
                    Price = (double)i.Price,
                });
            });

            return result;
        }

        private async Task<List<CatalogItem>> GetItemsByIdsAsync(List<int> ids)
        {
            if (ids.Count <= 0)
            {
                return new List<CatalogItem>();
            }

            var items = await catalogContext.CatalogItems.Where(ci => ids.Contains(ci.Id)).ToListAsync();
            items = ChangeUriPlaceholder(items);

            return items;
        }

        private List<CatalogItem> ChangeUriPlaceholder(List<CatalogItem> items)
        {
            foreach (CatalogItem item in items)
            {
                item.FillPicturePath(pictureBaseUrl!);
            }

            return items;
        }
    }
}