namespace Ordering.Infrastructure.EntityConfigurations
{
    public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("paymentmethods", OrderingContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseHiLo("paymentseq", OrderingContext.DefaultSchema);

            builder.Property<int>("BuyerId").IsRequired(true);

            builder.Property<string>("cardHolderName")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardHolderName")
                .HasMaxLength(200)
                .IsRequired(true);

            builder.Property<string>("alias")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Alias")
                .HasMaxLength(200)
                .IsRequired(true);

            builder.Property<string>("cardNumber")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardNumber")
                .HasMaxLength(25)
                .IsRequired(true);

            builder.Property<DateTime>("expiration")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Expiration")
                .HasMaxLength(25)
                .IsRequired(true);

            builder.Property<int>("cardTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardTypeId")
                .IsRequired(true);

            builder.HasOne(p => p.CardType)
                .WithMany()
                .HasForeignKey("cardTypeId");

            builder.Ignore(x => x.DomainEvents);
        }
    }
}