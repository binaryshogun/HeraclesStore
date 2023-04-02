var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomMVC()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddCustomDbContext(builder.Configuration)
    .AddCustomIdentity();

builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
