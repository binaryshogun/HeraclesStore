namespace Identity.UnitTests.Mocks
{
    public static class IdentityMockFactory
    {
        public static Mock<IUsersRepository> CreateUsersRepositoryMock(List<IdentityUser> users)
        {
            var repository = new Mock<IUsersRepository>();

            repository
                .Setup(r => r.GetByEmail(It.IsAny<string>()))
                .Returns((string email) => users.FirstOrDefault(u => u.Email == email));

            repository
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .Returns((string email) => Task.FromResult(users.FirstOrDefault(u => u.Email == email)));

            repository.Setup(r => r.SaveChanges());
            repository.Setup(r => r.SaveChangesAsync());

            return repository;
        }

        public static Mock<ITokenService> CreateTokenServiceMock()
        {
            var service = new Mock<ITokenService>();

            service
                .Setup(s => s.CreateToken(It.IsAny<IdentityUser>()))
                .Returns((IdentityUser user) => string.Join('.', user.Id, user.Email?.ToString(), user.UserName?.ToString()));

            return service;
        }

        public static Mock<UserManager<IdentityUser>> CreateUserManagerMock(List<IdentityUser> users)
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var manager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            manager.Object.UserValidators.Add(new UserValidator<IdentityUser>());
            manager.Object.PasswordValidators.Add(new PasswordValidator<IdentityUser>());

            manager
                .Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns((IdentityUser user, string password) =>
                {
                    user.Id = Guid.NewGuid().ToString();
                    user.PasswordHash = PasswordHelper.HashPassword(password);

                    return Task.FromResult(IdentityResult.Success);
                });

            manager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .Returns((string email) => Task.FromResult(users.FirstOrDefault(u => u.Email == email)));

            manager
                .Setup(m => m.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns((IdentityUser user, string password) => Task.FromResult(user.PasswordHash == PasswordHelper.HashPassword(password)));

            return manager;
        }

        public static Mock<ILogger<T>> CreateLoggerMock<T>()
        {
            var logger = new Mock<ILogger<T>>();

            logger.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ));

            return logger;
        }
    }
}