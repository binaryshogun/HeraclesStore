namespace Ordering.Api
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
            services
                .AddControllers(options => options.Filters.Add(typeof(HttpGlobalExceptionFilter)))
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

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
            services.AddDbContext<OrderingContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("OrderingDatabase"),
                    sqlServerOptions =>
                    {
                        sqlServerOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                        sqlServerOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    });
                options.LogTo(Log.Logger.Information, LogLevel.Information);
            });

            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("OrderingDatabase"),
                    sqlServerOptions =>
                    {
                        sqlServerOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
                        sqlServerOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    });
                options.LogTo(Log.Logger.Information, LogLevel.Information);
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
                .AddSqlServer(configuration.GetConnectionString("OrderingDatabase")!,
                    name: "Ordering-Database-check",
                    tags: new string[] { "orderingdatabase" })
                .AddRabbitMQ(
                    $"amqp://{configuration.GetConnectionString("EventBus")}",
                    name: "Ordering-EventBus-check"
                );

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
                    Title = "Heracles store - Ordering HTTP API",
                    Version = "v1",
                    Description = "The Ordering Service HTTP API"
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWT").GetValue<string>("SecurityKey")!)),
                    };
                });

            return services;
        }

        /// <summary>
        /// Adds infrastructure for Integration Events.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <param name="configuration">Application's configuration.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(_ => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration.GetConnectionString("EventBus"),
                    Port = AmqpTcpEndpoint.UseDefaultPort,
                    VirtualHost = ConnectionFactory.DefaultVHost,
                    DispatchConsumersAsync = true
                };

                var username = configuration.GetSection("EventBus").GetValue<string?>("UserName");
                var password = configuration.GetSection("EventBus").GetValue<string?>("Password");
                var retryCount = configuration.GetSection("EventBus").GetValue<int>("RetryCount");

                if (!string.IsNullOrEmpty(username))
                {
                    factory.UserName = username;
                }

                if (!string.IsNullOrEmpty(password))
                {
                    factory.Password = password;
                }

                if (retryCount <= 0)
                {
                    retryCount = 5;
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }

        /// <summary>
        /// Adds event bus messaging support to application's services.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <param name="configuration">Application's configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var subscriptionClientName = configuration.GetSection("EventBus").GetValue<string>("SubscriptionClientName");
                    var retryCount = configuration.GetSection("EventBus").GetValue<int>("RetryCount");
                    if (retryCount <= 0)
                    {
                        retryCount = 5;
                    }

                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();

                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubcriptionsManager, queueName: subscriptionClientName, retryCount: retryCount);
                })
                .AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }
}