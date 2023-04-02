namespace Identity.Api
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMVC(this IServiceCollection services)
        {
            services.AddControllers();

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UsersContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"), sqlOptions =>
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null));
            });

            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<UsersContext>();

            return services;
        }
    }
}