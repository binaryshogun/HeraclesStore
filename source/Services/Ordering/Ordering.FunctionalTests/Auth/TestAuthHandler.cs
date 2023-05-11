namespace Ordering.FunctionalTests.Auth
{
    public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
    {
        private readonly string userId;

        public TestAuthHandler(IOptionsMonitor<TestAuthHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            userId = options.CurrentValue.UserId;
        }

        public string UserId => userId;
        public string UserName => $"User #{UserId}";
        public const string AuthenticationScheme = "Test";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.NameIdentifier, UserId)
            };

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}