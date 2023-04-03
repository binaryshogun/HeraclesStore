namespace Identity.Api.Models
{
    public class SignInResponse
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;
    }
}