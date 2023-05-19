namespace EventBus.EventLogs.Utilities
{
    public class ResilentTransaction
    {
        private readonly DbContext context;

        private ResilentTransaction(DbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static ResilentTransaction New(DbContext context)
        {
            return new(context);
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                await action();
                await transaction.CommitAsync();
            });
        }
    }
}