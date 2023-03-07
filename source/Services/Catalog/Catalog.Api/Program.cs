var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
