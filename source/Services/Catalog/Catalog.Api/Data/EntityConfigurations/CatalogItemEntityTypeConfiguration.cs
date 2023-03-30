namespace Catalog.Api.Data.EntityConfigurations
{
    public class CatalogItemEntityTypeConfiguration
        : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
                .UseHiLo("catalog_hilo")
                .IsRequired(true);

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ci => ci.Description)
                .IsRequired(true)
                .HasMaxLength(500);

            builder.Property(ci => ci.Price)
                .IsRequired(true)
                .HasPrecision(8, 2);

            builder.Property(ci => ci.PictureFileName)
                .IsRequired(false);

            builder.Ignore(ci => ci.PictureUri);

            builder.HasOne(ci => ci.CatalogBrand)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogBrandId);

            builder.HasOne(ci => ci.CatalogType)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogTypeId);

            builder.Property(ci => ci.AvailableInStock)
                .IsRequired(true);

            builder.Property(ci => ci.RestockThreshold)
                .IsRequired(true);

            builder.Property(ci => ci.MaxStockThreshold)
                .IsRequired(true);

            builder.Property(ci => ci.OnReorder)
                .IsRequired(true);
        }
    }
}