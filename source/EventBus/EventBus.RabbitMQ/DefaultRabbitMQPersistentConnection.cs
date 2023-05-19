namespace EventBus.RabbitMQ
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection, IDisposable
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> logger;
        private readonly int retryCount;

        private IConnection? connection;
        public bool Disposed;

        private readonly object mutex = new();

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.retryCount = retryCount;
        }

        public bool IsConnected => connection is not null and { IsOpen: true } && !Disposed;

        public IModel? CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return connection!.CreateModel();
        }

        public bool TryConnect()
        {
            logger.LogInformation("[RabbitMQ] ---> RabbitMQ Client is trying to connect");

            lock (mutex)
            {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        logger.LogWarning(ex, "[RabbitMQ] ---> RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    }
                );

                policy.Execute(() =>
                {
                    connection = connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    connection!.ConnectionShutdown += OnConnectionShutdown;
                    connection!.CallbackException += OnCallbackException;
                    connection!.ConnectionBlocked += OnConnectionBlocked;

                    logger.LogInformation("[RabbitMQ] ---> RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    logger.LogCritical("[RabbitMQ] ---> FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }

        private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            if (Disposed) return;

            logger.LogWarning("[RabbitMQ] ---> A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            if (Disposed) return;

            logger.LogWarning("[RabbitMQ] ---> A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        private void OnConnectionShutdown(object? sender, ShutdownEventArgs reason)
        {
            if (Disposed) return;

            logger.LogWarning("[RabbitMQ] ---> A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }

        public void Dispose()
        {
            if (Disposed) return;

            Disposed = true;

            try
            {
                if (IsConnected)
                {
                    connection!.ConnectionShutdown -= OnConnectionShutdown;
                    connection!.CallbackException -= OnCallbackException;
                    connection!.ConnectionBlocked -= OnConnectionBlocked;
                    connection!.Dispose();
                }
            }
            catch (IOException ex)
            {
                logger.LogCritical(ex.ToString());
            }
        }
    }
}