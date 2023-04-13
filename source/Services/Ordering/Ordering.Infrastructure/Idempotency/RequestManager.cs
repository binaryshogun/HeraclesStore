namespace Ordering.Infrastructure.Idempotency
{
    /// <summary>
    /// Represents request manager for <see cref="ClientRequest" />.
    /// </summary>
    public class RequestManager : IRequestManager
    {
        private readonly OrderingContext context;

        /// <summary>
        /// Initializes new instance of <see cref="RequestManager" /> with given <paramref name="context" />.
        /// </summary>
        /// <param name="context">Current <see cref="OrderingContext" />.</param>
        public RequestManager(OrderingContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var request = await context.FindAsync<ClientRequest>(id);

            return request is not null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var request = await ExistsAsync(id) ?
                throw new OrderingDomainException($"Request with {id} already exists") :
                new ClientRequest()
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            context.Add(request);

            await context.SaveChangesAsync();
        }
    }
}