namespace Identity.Api.Data
{
    public class UsersContext : IdentityUserContext<IdentityUser>
    {
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}