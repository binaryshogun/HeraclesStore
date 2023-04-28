namespace Catalog.FunctionalTests.Data
{
    public class CatalogTestContext : CatalogContext
    {
        public CatalogTestContext(DbContextOptions<CatalogContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}