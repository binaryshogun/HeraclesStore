namespace Identity.Api.Services
{
    public class TokenService : ITokenService
    {
        private const int ExpirationMinutes = 120;

        private readonly IConfiguration configuration;
        private readonly ILogger<TokenService> logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public string CreateToken(IdentityUser user)
        {
            logger.LogInformation("[IdentityApi] ---> Generating JWT token...");

            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);

            var token = BuildToken(CreateClaims(user), CreateSigningCredentials(), expiration);
            var tokenHandler = new JwtSecurityTokenHandler();

            logger.LogInformation("[IdentityApi] ---> Token successfully generated");

            return tokenHandler.WriteToken(token);
        }

        private JwtSecurityToken BuildToken(List<Claim>? claims, SigningCredentials credentials, DateTime expiration)
        {
            return new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );
        }

        private List<Claim>? CreateClaims(IdentityUser user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!)
                };

                return claims;
            }
            catch (Exception ex)
            {
                logger.LogInformation("[IdentityApi] ---> Exception occurred during JWT token generation: {exception} - {message}", ex, ex.Message);
            }

            return null;
        }
        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        Environment.GetEnvironmentVariable("JWT_SECURITYKEY")!
                    )
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}