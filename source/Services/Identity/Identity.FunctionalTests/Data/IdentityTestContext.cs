namespace Identity.FunctionalTests.Data
{
    public class IdentityTestContext : UsersContext
    {
        public IdentityTestContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }
    }
}