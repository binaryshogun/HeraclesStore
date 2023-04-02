namespace Identity.Api.Services
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user);
    }
}