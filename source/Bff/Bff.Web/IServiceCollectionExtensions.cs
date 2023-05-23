namespace Bff.Web
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds exception filter to controllers and sets up CORS policy for application.
        /// </summary>
        /// <param name="services">Application's services.</param>
        /// <returns>Configured application's <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

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
                    Title = "Heracles store - Web BFF aggregator",
                    Version = "v1",
                    Description = "Web BFF aggregator for services"
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
                .AddUrlGroup(new Uri($"{configuration.GetSection("Urls").GetValue<string>("Identity")}/hc"), name: "catalogapi-check", tags: new string[] { "catalogapi" })
                .AddUrlGroup(new Uri($"{configuration.GetSection("Urls").GetValue<string>("Catalog")}/hc"), name: "catalogapi-check", tags: new string[] { "catalogapi" })
                .AddUrlGroup(new Uri($"{configuration.GetSection("Urls").GetValue<string>("Basket")}/hc"), name: "catalogapi-check", tags: new string[] { "catalogapi" })
                .AddUrlGroup(new Uri($"{configuration.GetSection("Urls").GetValue<string>("Ordering")}/hc"), name: "catalogapi-check", tags: new string[] { "catalogapi" });

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

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHttpClient<IOrderingApiClient, OrderingApiClient>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            return services;
        }

        public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<GrpcExceptionInterceptor>();
            services
                .AddScoped<ICatalogService, CatalogService>()
                .AddScoped<IBasketService, BasketService>()
                .AddScoped<IOrderingService, OrderingService>();

            services
                .AddGrpcClient<GrpcBasket.GrpcBasketClient>((_, options) =>
                {
                    var basketApi = configuration.GetSection("Urls").GetValue<string>("Basket");
                    options.Address = new Uri(basketApi!);
                })
                .AddInterceptor<GrpcExceptionInterceptor>();

            services
                .AddGrpcClient<GrpcCatalog.GrpcCatalogClient>((_, options) =>
                {
                    var catalogApi = configuration.GetSection("Urls").GetValue<string>("Catalog");
                    options.Address = new Uri(catalogApi!);
                })
                .AddInterceptor<GrpcExceptionInterceptor>();

            services
                .AddGrpcClient<GrpcOrdering.GrpcOrderingClient>((services, options) =>
                {
                    var orderingApi = configuration.GetSection("Urls").GetValue<string>("Ordering");
                    options.Address = new Uri(orderingApi!);
                })
                .AddInterceptor<GrpcExceptionInterceptor>();

            return services;
        }
    }
}