namespace Ordering.Infrastructure.EntityConfigurations
{
    public class ClientRequestEntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest>
    {
        public void Configure(EntityTypeBuilder<ClientRequest> builder)
        {
            builder.ToTable("requests", OrderingContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired(true);
            builder.Property(x => x.Time).IsRequired(true);
        }
    }
}