namespace Catalog.Api.Data.EntityConfigurations
{
    public class CatalogTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<CatalogType>
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Id)
                .UseHiLo("catalog_type_hilo")
                .IsRequired(true);

            builder.Property(ct => ct.Type)
                .IsRequired(true)
                .HasMaxLength(100);
        }
    }
}