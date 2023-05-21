namespace Ordering.Api.Application.Commands
{
    public class SetStockRejectedOrderStatusIdentifiedCommandHandler : IdentifiedCommandHandler<SetStockRejectedOrderStatusCommand, bool>
    {
        public SetStockRejectedOrderStatusIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<SetStockRejectedOrderStatusCommand, bool>> logger
        ) : base(mediator, requestManager, logger) { }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}