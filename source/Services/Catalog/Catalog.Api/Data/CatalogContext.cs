namespace Catalog.Api.Data
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options)
            : base(options) { }

        public DbSet<CatalogItem> CatalogItems { get; set; } = default!;
        public DbSet<CatalogBrand> CatalogBrands { get; set; } = default!;
        public DbSet<CatalogType> CatalogTypes { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
        }
    }
}