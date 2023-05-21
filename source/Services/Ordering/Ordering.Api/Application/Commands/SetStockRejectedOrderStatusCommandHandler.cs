namespace Ordering.Api.Application.Commands
{
    public class SetStockRejectedOrderStatusCommandHandler : IRequestHandler<SetStockRejectedOrderStatusCommand, bool>
    {
        private readonly IOrderRepository repository;

        public SetStockRejectedOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            repository = orderRepository;
        }

        public async Task<bool> Handle(SetStockRejectedOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // Simulate a work time for rejecting the stock
            await Task.Delay(10000, cancellationToken);

            var orderToUpdate = await repository.GetAsync(request.OrderId);
            if (orderToUpdate is null)
            {
                return false;
            }

            orderToUpdate.SetCancelledStatusWhenStockIsRejected(request.OrderStockItems);
            return await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}