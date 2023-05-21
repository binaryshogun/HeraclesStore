namespace Ordering.Api.Application.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository repository;
        private readonly IMediator mediator;
        private readonly IOrderingIntegrationEventService orderingIntegrationEventService;
        private readonly ILogger<CreateOrderCommandHandler> logger;

        public CreateOrderCommandHandler(IMediator mediator,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            IOrderRepository repository,
            ILogger<CreateOrderCommandHandler> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(request.UserId);
            await orderingIntegrationEventService.AddAndSaveEventAsync(orderStartedIntegrationEvent);

            var address = new Address(request.Street!, request.City!, request.State!, request.Country!, request.ZipCode!);
            var order = new Order(request.UserId, request.Username!, address, request.CardTypeId, request.CardNumber!, request.CardSecurityNumber!, request.CardHolderName!, request.CardExpiration);

            foreach (var item in request.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName!, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }

            logger.LogInformation("[Ordering] ---> Creating Order: {@Order}", order);

            repository.Add(order);

            return await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}