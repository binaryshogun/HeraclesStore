namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IRabbitMQPersistentConnection persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> logger;
        private readonly IEventBusSubscriptionsManager subscriptionsManager;
        private readonly IServiceProvider serviceProvider;
        private readonly int retryCount;

        private IModel? consumerChannel;
        private string exchangeName;
        private string? queueName;

        public EventBusRabbitMQ(
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<EventBusRabbitMQ> logger,
            IServiceProvider serviceProvider,
            IEventBusSubscriptionsManager subscriptionsManager,
            string exchangeName = "event_bus_exchange",
            string? queueName = null,
            int retryCount = 5)
        {
            this.persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.subscriptionsManager = subscriptionsManager ?? new InMemoryEventBusSubscriptionsManager();

            this.exchangeName = exchangeName;
            this.queueName = queueName;

            this.serviceProvider = serviceProvider;
            this.retryCount = retryCount;

            consumerChannel = CreateConsumerChannel();

            this.subscriptionsManager.OnEventRemoved += SubscriptionsManager_OnEventRemoved;
        }

        public void PublishEvent(IntegrationEvent @event)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    logger.LogWarning(ex, "[RabbitMQ] ---> Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = @event.GetType().Name;

            logger.LogTrace("[RabbitMQ] ---> Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            using var channel = persistentConnection.CreateModel();
            logger.LogTrace("[RabbitMQ] ---> Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

            channel.ExchangeDeclare(exchangeName, type: "direct");

            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });

            policy.Execute(() =>
            {
                var properties = channel!.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent delivery mode

                logger.LogTrace("[RabbitMQ] ---> Publishing event to RabbitMQ: {EventId}", @event.Id);

                channel.BasicPublish(exchangeName, eventName, true, properties, body);
            });
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = subscriptionsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            logger.LogInformation("[RabbitMQ] ---> Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

            subscriptionsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        public void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            logger.LogInformation("[RabbitMQ] ---> Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

            DoInternalSubscription(eventName);
            subscriptionsManager.AddDynamicSubscription<TH>(eventName);
            StartBasicConsume();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = subscriptionsManager.GetEventKey<T>();

            logger.LogInformation("[RabbitMQ] ---> Unsubscribing from event {EventName}", eventName);

            subscriptionsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            subscriptionsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = subscriptionsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                consumerChannel?.QueueBind(queueName, exchangeName, eventName);
            }
        }

        private void StartBasicConsume()
        {
            logger.LogTrace("[RabbitMQ] ---> Starting RabbitMQ basic consume");

            if (consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(consumerChannel);

                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(queueName, false, consumer);
            }
            else
            {
                logger.LogError("[RabbitMQ] ---> Basic consume cannot be started because consumer channel is not set");
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            logger.LogTrace("[RabbitMQ] ---> Creating RabbitMQ consumer channel");

            var channel = persistentConnection.CreateModel();

            channel?.ExchangeDeclare(exchangeName, "direct");

            channel?.QueueDeclare(queueName, true, false, false, null);

            channel!.CallbackException += (sender, ea) =>
            {
                logger.LogWarning(ea.Exception, "[RabbitMQ] ---> Recreating RabbitMQ consumer channel");

                consumerChannel?.Dispose();
                consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            logger.LogTrace("[RabbitMQ] ---> Processing RabbitMQ event: {EventName}", eventName);

            if (subscriptionsManager.HasSubscriptionsForEvent(eventName))
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var subscriptions = subscriptionsManager.GetHandlersForEvent(eventName);

                foreach (SubscriptionInfo? subscription in subscriptions)
                {
                    if (subscription is not null && subscription.IsDynamic)
                    {
                        if (scope.ServiceProvider.GetService(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler)
                        {
                            continue;
                        }

                        using dynamic eventData = JsonDocument.Parse(message);

                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else if (subscription is not null)
                    {
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        if (handler == null)
                        {
                            continue;
                        };

                        var eventType = subscriptionsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType!, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType!);

                        await Task.Yield();
                        await (concreteType.GetMethod("Handle")?.Invoke(handler, new object?[] { integrationEvent }) as Task)!;
                    }
                }
            }
            else
            {
                logger.LogWarning("[RabbitMQ] ---> No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[RabbitMQ] ---> ERROR Processing message \"{Message}\"", message);
            }

            consumerChannel?.BasicAck(eventArgs.DeliveryTag, false);
        }

        private void SubscriptionsManager_OnEventRemoved(object? sender, string eventName)
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            using var channel = persistentConnection.CreateModel();
            channel.QueueUnbind(queueName, exchangeName, eventName);

            if (subscriptionsManager.IsEmpty)
            {
                queueName = string.Empty;
                consumerChannel?.Close();
            }
        }

        public void Dispose()
        {
            if (consumerChannel != null)
            {
                consumerChannel.Dispose();
            }

            subscriptionsManager.Clear();
        }

    }
}