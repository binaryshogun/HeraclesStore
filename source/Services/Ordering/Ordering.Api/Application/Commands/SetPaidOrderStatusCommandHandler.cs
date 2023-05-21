namespace Ordering.Api.Application.Commands
{
    public class SetPaidOrderStatusCommandHandler : IRequestHandler<SetPaidOrderStatusCommand, bool>
    {
        private readonly IOrderRepository repository;

        public SetPaidOrderStatusCommandHandler(IOrderRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> Handle(SetPaidOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // Simulate a work time for validating the payment
            await Task.Delay(10000, cancellationToken);

            var orderToUpdate = await repository.GetAsync(request.OrderId);
            if (orderToUpdate is null)
            {
                return false;
            }

            orderToUpdate.SetPaidStatus();
            return await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}