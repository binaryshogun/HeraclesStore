namespace Ordering.Api.Application.Events.Domain
{
    public class BuyerPaymentMethodVerifiedDomainEventHandler
        : INotificationHandler<BuyerPaymentMethodVerifiedDomainEvent>
    {
        private readonly IOrderRepository repository;
        private readonly ILogger<BuyerPaymentMethodVerifiedDomainEventHandler> logger;

        public BuyerPaymentMethodVerifiedDomainEventHandler(
            IOrderRepository repository,
            ILogger<BuyerPaymentMethodVerifiedDomainEventHandler> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(BuyerPaymentMethodVerifiedDomainEvent notification, CancellationToken cancellationToken)
        {
            var orderToUpdate = await repository.GetAsync(notification.OrderId);

            if (orderToUpdate is not null)
            {
                orderToUpdate.BuyerId = notification.Buyer.Id;
                orderToUpdate.PaymentMethodId = notification.PaymentMethod.Id;

                logger.LogInformation("[Ordering] ---> Order with Id: {OrderId} has been successfully updated with a payment method {PaymentMethod} ({PaymentId})",
                    notification.OrderId, notification.PaymentMethod, notification.PaymentMethod.Id);
            }
        }
    }
}