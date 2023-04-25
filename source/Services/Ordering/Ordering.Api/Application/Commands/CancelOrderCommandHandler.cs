namespace Ordering.Api.Application.Commands
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IOrderRepository repository;

        public CancelOrderCommandHandler(IOrderRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await repository.GetAsync(request.OrderId);

            if (order is null)
            {
                return false;
            }

            order.SetCancelledStatus();
            return await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}