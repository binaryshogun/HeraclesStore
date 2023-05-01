namespace Basket.FunctionalTests.Services
{
    internal class BasketWebHostFactory : WebApplicationFactory<Program>
    {
        public static string DefaultUserId { get; set; } = Guid.NewGuid().ToString();

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var path = Assembly.GetAssembly(typeof(BasketWebHostFactory))?.Location;

            builder
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", false);
                });

            Environment.SetEnvironmentVariable("JWT_SECURITYKEY", "SomethingSecret!");

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<TestAuthHandlerOptions>(options => options.UserId = DefaultUserId);

                services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
            });
        }
    }
}