namespace Ordering.Infrastructure.EntityConfigurations
{
    public class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.ToTable("buyers", OrderingContext.DefaultSchema);

            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.IdentityId).IsUnique(true);

            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Id).UseHiLo("buyerseq", OrderingContext.DefaultSchema);
            builder.Property(x => x.IdentityId).HasMaxLength(1000).IsRequired(true);
            builder.Property(x => x.Name).IsRequired(true);

            builder.Property(x => x.PaymentMethods).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(x => x.PaymentMethods)
                .WithOne()
                .HasForeignKey("BuyerId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}