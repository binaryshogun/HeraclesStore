namespace Catalog.Api.Data.EntityConfigurations
{
    public class CatalogBrandEntityTypeConfiguration
        : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand");

            builder.HasKey(cb => cb.Id);

            builder.Property(cb => cb.Id)
                .UseHiLo("catalog_brand_hilo")
                .IsRequired(true);

            builder.Property(cb => cb.Brand)
                .IsRequired(true)
                .HasMaxLength(100);
        }
    }
}