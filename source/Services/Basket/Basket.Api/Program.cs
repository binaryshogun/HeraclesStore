var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomMVC()
    .AddEndpointsApiExplorer()
    .AddCustomConfiguration()
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomHealthCheck(builder.Configuration)
    .AddCustomRedis(builder.Configuration)
    .AddIntegrationServices(builder.Configuration)
    .AddEventBus(builder.Configuration)
    .AddCustomSwaggerGen();

builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddGrpcReflection();

builder.Configuration.AddEnvironmentVariables();

builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();

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
    .WriteTo.File("../../../Logs/basket-api-.txt", rollingInterval: RollingInterval.Hour)
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

app.MapGrpcService<BasketService>();
app.MapGet("/proto", async context =>
{
    context.Response.ContentType = "text/plain";
    using var fileStream = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
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

EventBusConfigurator.ConfigureEventBus(app);

app.Run();
