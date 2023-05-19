namespace EventBus.RabbitMQ
{
    public interface IRabbitMQPersistentConnection
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel? CreateModel();
    }
}