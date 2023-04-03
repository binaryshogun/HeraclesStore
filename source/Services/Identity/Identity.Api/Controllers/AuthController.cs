namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IUsersRepository repository;
        private readonly ITokenService tokenService;
        private readonly ILogger<AuthController> logger;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IUsersRepository repository,
            ITokenService tokenService,
            ILogger<AuthController> logger)
        {
            this.userManager = userManager;
            this.repository = repository;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(SignUpResponse), StatusCodes.Status201Created, "application/json")]
        public async Task<ActionResult> SignUp(SignUpRequest request)
        {
            var result = await userManager.CreateAsync(
                new IdentityUser { UserName = request.Username, Email = request.Email },
                request.Password!
            );

            if (!result.Succeeded)
            {
                throw new IdentityException(string.Join(';', result.Errors));
            }

            return CreatedAtAction(nameof(SignUp), new SignUpResponse { Username = request.Username!, Email = request.Email! });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(SignInResponse), StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized, "application/json")]
        public async Task<ActionResult<SignInResponse>> SignIn(SignInRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email!);

            if (user is null)
            {
                return Unauthorized("User not found");
            }

            var isValid = await userManager.CheckPasswordAsync(user, request.Password!);

            if (!isValid)
            {
                return Unauthorized("Wrong credentials");
            }

            // Generate access token for the specific user in database
            var userInDatabase = await repository.GetByEmailAsync(request.Email);

            if (userInDatabase is null)
            {
                return Unauthorized("User not found");
            }

            var accessToken = tokenService.CreateToken(userInDatabase);

            // Save user token
            await repository.SaveChangesAsync();

            return new SignInResponse()
            {
                Username = userInDatabase.UserName!,
                Email = userInDatabase.Email!,
                Token = accessToken
            };
        }
    }
}