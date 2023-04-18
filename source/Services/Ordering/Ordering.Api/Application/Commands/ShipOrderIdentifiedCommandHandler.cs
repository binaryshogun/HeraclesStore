namespace Ordering.Api.Application.Commands
{
    public class ShipOrderIdentifiedCommandHandler : IdentifiedCommandHandler<ShipOrderCommand, bool>
    {
        public ShipOrderIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager, ILogger<IdentifiedCommandHandler<ShipOrderCommand, bool>> logger)
            : base(mediator, requestManager, logger) { }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}