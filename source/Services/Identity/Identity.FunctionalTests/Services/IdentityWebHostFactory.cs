namespace Identity.FunctionalTests.Services
{
    internal class IdentityWebHostFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var path = Assembly.GetAssembly(typeof(IdentityWebHostFactory))?.Location;

            builder
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((HostBuilderContext hostContext, IServiceCollection services) =>
                {
                    // Remove the app's ApplicationDbContext registration.
                    var optionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UsersContext>));

                    if (optionsDescriptor != null)
                    {
                        services.Remove(optionsDescriptor);
                    }

                    var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(UsersContext));

                    if (contextDescriptor != null)
                    {
                        services.Remove(contextDescriptor);
                    }

                    var optionsBuilder = new DbContextOptionsBuilder<UsersContext>();
                    optionsBuilder.UseInMemoryDatabase("IdentityDb");

                    services.AddScoped<DbContextOptions<UsersContext>>((sp) => optionsBuilder.Options);
                    services.AddDbContext<UsersContext, IdentityTestContext>();
                });

            Environment.SetEnvironmentVariable("JWT_SECURITYKEY", "SomethingSecret!");

            return base.CreateHost(builder);
        }
    }
}