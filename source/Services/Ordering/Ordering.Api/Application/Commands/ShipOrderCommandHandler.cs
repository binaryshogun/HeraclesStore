namespace Ordering.Api.Application.Commands
{
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, bool>
    {
        private readonly IOrderRepository repository;

        public ShipOrderCommandHandler(IOrderRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await repository.GetAsync(request.OrderId);

            if (order is null)
            {
                return false;
            }

            order.SetShippedStatus();
            return await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}