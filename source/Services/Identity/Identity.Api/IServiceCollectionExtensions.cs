namespace Identity.Api
{
    /// <summary>
    /// Extensions class that contains static methods to set up application's service collection.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds exception filter to controllers and sets up CORS policy for application.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomMVC(this IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.Add(typeof(HttpGlobalExceptionFilter)));
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }

        /// <summary>
        /// Setting up <see cref="DbContext" /> for <paramref name="services" />.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <param name="configuration">Application's configuration.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UsersContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"), sqlOptions =>
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null));
            });

            return services;
        }

        /// <summary>
        /// Setting up <see cref="ApiBehaviorOptions" /> to react on invalid model state.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional information"
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }

        /// <summary>
        /// Adds health checks for application, databases and other application's services.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <param name="configuration">Application's configuration.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var healthCheckBuilder = services.AddHealthChecks();

            healthCheckBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddSqlServer(configuration.GetConnectionString("CatalogDb")!,
                    name: "CatalogDb-check",
                    tags: new string[] { "catalogdatabase" });

            // TODO: RabbitMQ health check

            return services;
        }

        /// <summary>
        /// Setting up identity core for application's services.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
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