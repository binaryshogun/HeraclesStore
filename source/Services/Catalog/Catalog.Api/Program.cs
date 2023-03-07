var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Set up serilog as logging provider
builder.Host.UseSerilog((hostContext, services, configuration) => 
{
    configuration.WriteTo.Console();
    configuration.WriteTo.File($"Logs\\log.txt", rollingInterval: RollingInterval.Hour, retainedFileCountLimit: null);
    configuration.ReadFrom.Configuration(builder.Configuration);
});

// Inject DbContext to the services container
builder.Services.AddDbContext<CatalogContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb"),
        sqlServerOptions => 
        {
            sqlServerOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await new SeedData().EnsurePopulated(app);

app.Logger.LogInformation($"Starting web application {app.Environment.ApplicationName}...");

await app.StartAsync();

app.Logger.LogInformation($"Now listening on: \n\t\t{string.Join("\n\t\t", app.Urls)}");

await app.WaitForShutdownAsync();