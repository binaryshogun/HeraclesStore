namespace Identity.Api.Data
{
    public interface IUsersRepository
    {
        IdentityUser? GetByEmail(string? email);
        Task<IdentityUser?> GetByEmailAsync(string? email);

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}