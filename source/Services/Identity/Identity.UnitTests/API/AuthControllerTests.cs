namespace Identity.UnitTests.API
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<IdentityUser>> userManager;
        private readonly Mock<IUsersRepository> repository;
        private readonly Mock<ITokenService> tokenService;
        private readonly Mock<ILogger<AuthController>> logger;

        public AuthControllerTests()
        {
            var users = GetDefaultUsers();

            userManager = IdentityMockFactory.CreateUserManagerMock(users);
            repository = IdentityMockFactory.CreateUsersRepositoryMock(users);
            tokenService = IdentityMockFactory.CreateTokenServiceMock();
            logger = IdentityMockFactory.CreateLoggerMock<AuthController>();
        }

        [Fact]
        public async Task SignUp_ResponseStatusShouldBeStatus200OK()
        {
            // Given
            var controller = new AuthController(userManager.Object, repository.Object, tokenService.Object, logger.Object);
            var request = new SignUpRequest()
            {
                Username = "NewUser",
                Email = "newuser@example.com",
                Password = "newpassword",
            };

            // When
            var response = await controller.SignUp(request);

            // Then
            var result = Assert.IsType<CreatedAtActionResult>(response);

            var created = Assert.IsType<SignUpResponse>(result.Value);
            Assert.NotNull(created);
            Assert.Equal(request.Username, created.Username);
            Assert.Equal(request.Email, created.Email);
        }

        [Fact]
        public async Task SignIn_CorrectCredentials_ResponseStatusShouldBeStatus200OK()
        {
            // Given
            var controller = new AuthController(userManager.Object, repository.Object, tokenService.Object, logger.Object);
            var request = new SignInRequest()
            {
                Email = "user1@example.com",
                Password = "password1",
            };

            // When
            var response = await controller.SignIn(request);

            // Then
            var result = Assert.IsType<ActionResult<SignInResponse>>(response);

            var signInResponse = Assert.IsType<SignInResponse>(result.Value);
            Assert.NotNull(signInResponse);
            Assert.NotNull(signInResponse.Token);
            Assert.NotNull(signInResponse.Username);
            Assert.Equal(request.Email, signInResponse.Email);
        }

        [Fact]
        public async Task SignIn_WrongCredentials_ResponseStatusShouldBeStatus401Unauthorized()
        {
            // Given
            var controller = new AuthController(userManager.Object, repository.Object, tokenService.Object, logger.Object);
            var request = new SignInRequest()
            {
                Email = "wronguser@example.com",
                Password = "wrongpassword",
            };

            // When
            var response = await controller.SignIn(request);

            // Then
            var result = Assert.IsType<ActionResult<SignInResponse>>(response);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Null(result.Value);
        }

        private List<IdentityUser> GetDefaultUsers()
        {
            return new()
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "User1",
                    Email = "user1@example.com",
                    PasswordHash = PasswordHelper.HashPassword("password1")
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "User2",
                    Email = "user2@example.com",
                    PasswordHash = PasswordHelper.HashPassword("password2")
                }
            };
        }
    }
}