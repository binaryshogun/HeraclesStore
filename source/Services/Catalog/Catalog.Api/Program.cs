var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomMVC()
    .AddEndpointsApiExplorer()
    .AddCustomDbContext(builder.Configuration)
    .AddCustomConfiguration()
    .AddSwaggerGen()
    .AddCustomHealthCheck(builder.Configuration);

builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
builder.Environment.WebRootPath = "Pictures";

// Configuring Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("../../../Logs/catalog-api-.txt", rollingInterval: RollingInterval.Hour)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Using Serilog as logging provider
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
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

await SeedData.EnsurePopulated(app);

app.Run();