namespace Ordering.Api.Infrastructure
{
    public class IntegrationEventLogContextSeed
    {
        public static void MigrateContext(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                Migrate(
                    scope.ServiceProvider.GetRequiredService<IntegrationEventLogContext>(),
                    scope.ServiceProvider.GetRequiredService<ILogger<IntegrationEventLogContextSeed>>());
            }
        }

        private static void Migrate(IntegrationEventLogContext context, ILogger<IntegrationEventLogContextSeed> logger)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation($"---> Applying migrations...");
                context.Database.Migrate();
            }
        }
    }
}