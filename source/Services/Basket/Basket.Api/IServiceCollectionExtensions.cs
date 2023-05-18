namespace Basket.Api
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
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var healthCheckBuilder = services.AddHealthChecks();

            healthCheckBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddRedis(configuration.GetValue<string>("RedisConnection")!,
                    name: "Redis-check",
                    tags: new string[] { "redis" });

            // TODO: RabbitMQ health check

            return services;
        }

        /// <summary>
        /// Adds Swagger Gen and Open API definitions to the application's services.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Heracles store - Basket HTTP API",
                    Version = "v1",
                    Description = "The Basket Service HTTP API"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please, enter a valid JWT token",
                    Name = "Auth",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// Adds authentication with JWT bearer tokens to the application's services.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <param name="configuration">Application's configuration.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetSection("JWT").GetValue<string>("Issuer"),
                        ValidAudience = configuration.GetSection("JWT").GetValue<string>("Issuer"),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                Environment.GetEnvironmentVariable("JWT_SECURITYKEY")!
                            )
                        ),
                    };
                });

            return services;
        }

        /// <summary>
        /// Adds redis support by setting up <see cref="ConnectionMultiplexer" /> to the application's services.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <param name="configuration">Application's configuration.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ConnectionMultiplexer>(serviceProvider =>
            {
                var connectionString = configuration.GetValue<string>("RedisConnection");
                var options = ConfigurationOptions.Parse(connectionString!, true);

                return ConnectionMultiplexer.Connect(options);
            });

            return services;
        }
    }
}