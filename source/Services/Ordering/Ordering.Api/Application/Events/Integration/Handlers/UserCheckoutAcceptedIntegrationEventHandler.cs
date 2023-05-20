namespace Ordering.Api.Application.Events.Integration.Handlers
{
    public class UserCheckoutAcceptedIntegrationEventHandler : IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>
    {
        private readonly IMediator mediator;
        private readonly ILogger<UserCheckoutAcceptedIntegrationEventHandler> logger;

        public UserCheckoutAcceptedIntegrationEventHandler(
            IMediator mediator,
            ILogger<UserCheckoutAcceptedIntegrationEventHandler> logger)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UserCheckoutAcceptedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}"))
            {
                logger.LogInformation("[Ordering] ---> Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

                var result = false;

                if (@event.RequestId != Guid.Empty)
                {
                    using (LogContext.PushProperty("IdentifiedCommandId", @event.RequestId))
                    {
                        var createOrderCommand =
                            new CreateOrderCommand(
                                @event.Basket.Items, @event.UserId, @event.UserName, @event.City, @event.Street,
                                @event.State, @event.Country, @event.ZipCode,
                                @event.CardNumber, @event.CardHolderName, @event.CardExpiration,
                                @event.CardSecurityNumber, @event.CardTypeId
                            );

                        var requestCreateOrder = new IdentifiedCommand<CreateOrderCommand, bool>(createOrderCommand, @event.RequestId);

                        logger.LogInformation(
                            "[Ordering] ---> Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                            requestCreateOrder.GetGenericTypeName(),
                            nameof(requestCreateOrder.CommandId),
                            requestCreateOrder.CommandId,
                            requestCreateOrder
                        );

                        result = await mediator.Send(requestCreateOrder);

                        if (result)
                        {
                            logger.LogInformation("[Ordering] ---> CreateOrderCommand suceeded - RequestId: {RequestId}", @event.RequestId);
                        }
                        else
                        {
                            logger.LogWarning("[Ordering] ---> CreateOrderCommand failed - RequestId: {RequestId}", @event.RequestId);
                        }
                    }
                }
                else
                {
                    logger.LogWarning("[Ordering] ---> Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}", @event);
                }
            }
        }
    }
}