namespace EventBus.EventLogs.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
    {
        private readonly IntegrationEventLogContext integrationEventLogContext;
        private readonly DbConnection dbConnection;
        private readonly List<Type> eventTypes;
        private volatile bool disposedValue;

        public IntegrationEventLogService(DbConnection dbConnection)
        {
            this.dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            this.integrationEventLogContext = new IntegrationEventLogContext(
                new DbContextOptionsBuilder<IntegrationEventLogContext>().UseSqlServer(dbConnection).Options);
            this.eventTypes = Assembly.Load(Assembly.GetEntryAssembly()?.FullName ?? throw new InvalidOperationException())
                .GetTypes().Where(t => t.Name.EndsWith(nameof(IntegrationEvent))).ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            var result = await integrationEventLogContext.IntegrationEventLogs
                .Where(e => e.TransactionId == tid && e.State == EventState.NotPublished).ToListAsync();

            if (result.Any())
            {
                return result.OrderBy(o => o.CreationTime).Select(e => e.DeserializeJsonContent(eventTypes?.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<IntegrationEventLogEntry>();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventState.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventState status)
        {
            var eventLogEntry = integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventState.InProgress)
                eventLogEntry.TimesSent++;

            integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

            return integrationEventLogContext.SaveChangesAsync();
        }

        public Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

            integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

            return integrationEventLogContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    integrationEventLogContext?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}