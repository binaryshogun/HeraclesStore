namespace Ordering.Infrastructure.EntityConfigurations
{
    public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("orderitems", OrderingContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseHiLo("orderitemseq", OrderingContext.DefaultSchema);

            builder.Property<int>("OrderId").IsRequired(true);
            builder.Property<int>("ProductId").IsRequired(true);

            builder.Property(x => x.Units)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Units")
                .IsRequired(true);

            builder.Property(x => x.UnitPrice)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("UnitPrice")
                .IsRequired(true);

            builder.Property(x => x.Discount)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Discount")
                .HasPrecision(8, 2)
                .HasDefaultValue(0)
                .IsRequired(true);

            builder.Property(x => x.ProductName)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ProductName")
                .IsRequired(true);

            builder.Property(x => x.PictureUrl)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PictureUrl")
                .IsRequired(false);

            builder.Ignore(x => x.DomainEvents);
        }
    }
}