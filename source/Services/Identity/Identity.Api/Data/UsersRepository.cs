namespace Identity.Api.Data
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UsersContext context;

        public UsersRepository(UsersContext context)
        {
            this.context = context;
        }

        public IdentityUser? GetByEmail(string? email)
        {
            return email is null ? null : context.Users.FirstOrDefault(u => u.Email == email);
        }

        public async Task<IdentityUser?> GetByEmailAsync(string? email)
        {
            return email is null ? null : await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}