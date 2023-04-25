namespace Ordering.Infrastructure.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders", OrderingContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseHiLo("orderseq", OrderingContext.DefaultSchema);

            builder.Property<DateTime>("orderDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderDate")
                .IsRequired(true);

            builder.Property<string?>("description")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Description")
                .IsRequired(true);

            builder.Property<int>("orderStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderStatusId")
                .IsRequired(true);

            builder.Property(x => x.BuyerId)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("BuyerId")
                .IsRequired(false);

            builder.Property(x => x.PaymentMethodId)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentMethodId")
                .IsRequired(false);

            builder.Navigation(x => x.OrderItems).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(x => x.DomainEvents);

            builder.OwnsOne(x => x.Address, adress =>
            {
                adress.Property<int>("OrderId").UseHiLo("orderseq", OrderingContext.DefaultSchema);
                adress.WithOwner();
            });

            builder.HasOne(x => x.OrderStatus).WithMany().HasForeignKey("orderStatusId").IsRequired(true);

            builder.HasOne<Buyer>().WithMany().HasForeignKey(x => x.BuyerId).IsRequired(false);
            builder.HasOne<PaymentMethod>().WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
        }
    }
}