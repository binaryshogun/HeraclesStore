namespace Ordering.Api.Application.Commands
{
    public class SetStockConfirmedOrderStatusCommandHandler : IRequestHandler<SetStockConfirmedOrderStatusCommand, bool>
    {
        private readonly IOrderRepository repository;

        public SetStockConfirmedOrderStatusCommandHandler(IOrderRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> Handle(SetStockConfirmedOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // Simulate a work time for confirming the stock
            await Task.Delay(10000, cancellationToken);

            var orderToUpdate = await repository.GetAsync(request.OrderId);
            if (orderToUpdate is null)
            {
                return false;
            }

            orderToUpdate.SetStockConfirmedStatus();
            return await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}