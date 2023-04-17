namespace Ordering.Api.Infrastructure
{
    public class OrderingContextSeed
    {
        public static async Task SeedAsync(OrderingContext context, ILogger<OrderingContextSeed> logger)
        {
            var policy = CreatePolicy(logger);

            await policy.ExecuteAsync(async () =>
            {
                using (context)
                {
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                    }

                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(GetPredefinedCardTypes());

                        await context.SaveChangesAsync();
                    }

                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(GetPredefinedOrderStatus());

                        await context.SaveChangesAsync();
                    }
                }
            });
        }

        private static IEnumerable<CardType> GetPredefinedCardTypes()
        {
            return Enumeration.GetAll<CardType>();
        }

        private static IEnumerable<OrderStatus> GetPredefinedOrderStatus()
        {
            return Enumeration.GetAll<OrderStatus>();
        }

        private static AsyncRetryPolicy CreatePolicy(ILogger<OrderingContextSeed> logger, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[Ordering] ---> Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}