namespace EventBus.EventLogs
{
    public class IntegrationEventLogEntry
    {
        private IntegrationEventLogEntry() { }

        public IntegrationEventLogEntry(IntegrationEvent @event, Guid transactionId)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName ?? "";
            JsonContent = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId.ToString();
        }

        public Guid EventId { get; private set; }

        public string? EventTypeName { get; private set; }

        [NotMapped]
        public string? EventTypeShortName => EventTypeName?.Split('.')?.Last();

        [NotMapped]
        public IntegrationEvent? IntegrationEvent { get; private set; }

        public EventState State { get; set; }

        public int TimesSent { get; set; }

        public DateTime CreationTime { get; private set; }

        public string? JsonContent { get; private set; }

        public string? TransactionId { get; private set; }

        public IntegrationEventLogEntry DeserializeJsonContent(Type? type)
        {
            if (JsonContent is not null && type is not null)
            {
                IntegrationEvent = JsonSerializer.Deserialize(JsonContent, type, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as IntegrationEvent;
            }

            return this;
        }
    }
}