var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomMVC()
    .AddEndpointsApiExplorer()
    .AddCustomDbContext(builder.Configuration)
    .AddCustomConfiguration()
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomSwaggerGen()
    .AddCustomHealthCheck(builder.Configuration)
    .AddSignalR();

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
        containerBuilder.RegisterModule(new ApplicationModule(
            builder.Configuration.GetConnectionString("OrderingDatabase") ?? ""));
    });

// Configuring Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("../../../Logs/ordering-api-.txt", rollingInterval: RollingInterval.Hour)
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

app.Run();
