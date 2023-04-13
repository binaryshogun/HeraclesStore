namespace Ordering.Infrastructure.EntityConfigurations
{
    public class CardTypeEntityTypeConfiguration : IEntityTypeConfiguration<CardType>
    {
        public void Configure(EntityTypeBuilder<CardType> builder)
        {
            builder.ToTable("cardtypes", OrderingContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasDefaultValue(1).ValueGeneratedNever();
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired(true);
        }
    }
}