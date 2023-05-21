var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomMVC()
    .AddEndpointsApiExplorer()
    .AddCustomDbContext(builder.Configuration)
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomIntegrations(builder.Configuration)
    .AddEventBus(builder.Configuration)
    .AddCustomHealthChecks(builder.Configuration)
    .AddCustomConfiguration()
    .AddCustomSwaggerGen()
    .AddSignalR();

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddGrpcReflection();

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        var mediatorConfiguration = MediatRConfigurationBuilder
            .Create(typeof(Program).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        containerBuilder.RegisterMediatR(mediatorConfiguration);
        containerBuilder.RegisterModule(new MediatorModule());
        containerBuilder.RegisterModule(new ApplicationModule(builder.Configuration.GetConnectionString("OrderingDatabase") ?? ""));
    });

builder.WebHost.UseKestrel(options =>
{
    var ports = NetworkConfigurator.GetDefinedPorts(builder.Configuration);

    options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// Configuring Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("../../../Logs/-ordering-api.txt", rollingInterval: RollingInterval.Hour)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Using Serilog as logging provider
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<OrderingService>();
app.MapGet("/proto", async context =>
{
    context.Response.ContentType = "text/plain";
    using var fileStream = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Proto", "ordering.proto"), FileMode.Open, FileAccess.Read);
    using var streamReader = new StreamReader(fileStream);

    while (!streamReader.EndOfStream)
    {
        var line = await streamReader.ReadLineAsync() ?? "";
        if (line != "/* >>" || line != "<< */")
        {
            await context.Response.WriteAsync(line);
        }
    }
});
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions()
{
    Predicate = r => r.Name.Contains("self")
});
app.MapHub<NotificationsHub>("/hub/notifications");

await OrderingContextSeed.SeedAsync(app);
EventBusConfiguratior.ConfigureEventBus(app);

app.Run();
