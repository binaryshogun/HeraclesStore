namespace Ordering.FunctionalTests.Data
{
    public class OrderingTestContext : OrderingContext
    {
        public OrderingTestContext(DbContextOptions<OrderingContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}