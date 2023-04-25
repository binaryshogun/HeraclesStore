namespace Ordering.Api.Application.Commands
{
    public class CancelOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CancelOrderCommand, bool>
    {
        public CancelOrderIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager, ILogger<IdentifiedCommandHandler<CancelOrderCommand, bool>> logger)
            : base(mediator, requestManager, logger) { }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}