namespace Ordering.Api.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly OrderingContext context;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> logger;

        public TransactionBehavior(OrderingContext context, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var type = request.GetType();

            try
            {
                if (context.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await context.BeginTransactionAsync())
                    {
                        if (transaction is not null)
                        {
                            logger.LogInformation("[Ordering] ---> Begin transaction {TransactionId} for {RequestName} ({@Request})", transaction.TransactionId, type, request);

                            response = await next();

                            logger.LogInformation("[Ordering] ---> Commit transaction {TransactionId} for {RequestName} ({@Request})", transaction.TransactionId, type, request);

                            await context.CommitTransactionAsync(transaction);
                        }
                    }
                });

                return response!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing transaction for {RequestName} ({@Request})", type, request);

                throw;
            }
        }
    }
}