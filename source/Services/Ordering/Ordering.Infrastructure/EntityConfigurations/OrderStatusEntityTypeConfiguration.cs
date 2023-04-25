namespace Ordering.Infrastructure.EntityConfigurations
{
    public class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.ToTable("orderstatus", OrderingContext.DefaultSchema);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).ValueGeneratedNever();
            builder.Property(o => o.Name).HasMaxLength(200).IsRequired(true);
        }
    }
}