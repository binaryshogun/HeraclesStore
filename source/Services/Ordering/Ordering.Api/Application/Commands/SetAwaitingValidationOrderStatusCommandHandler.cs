namespace Ordering.Api.Application.Commands
{
    public class SetAwaitingValidationOrderStatusCommandHandler : IRequestHandler<SetAwaitingValidationOrderStatusCommand, bool>
    {
        private readonly IOrderRepository repository;

        public SetAwaitingValidationOrderStatusCommandHandler(IOrderRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> Handle(SetAwaitingValidationOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await repository.GetAsync(request.OrderId);
            if (orderToUpdate is null)
            {
                return false;
            }

            orderToUpdate.SetAwaitingValidationStatus();
            return await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}