namespace Catalog.Api.Profiles
{
    public class CatalogItemProfile : Profile
    {
        public CatalogItemProfile()
        {
            // Source --> Target
            CreateMap<CatalogItem, CatalogItemReadDto>();
            CreateMap<CatalogItemCreateDto, CatalogItem>();
            CreateMap<CatalogItemUpdateDto, CatalogItem>();
        }
    }
}