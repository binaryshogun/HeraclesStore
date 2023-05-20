namespace Ordering.Api.Application.Commands
{
    public class SetPaidOrderStatusIdentifiedCommandHandler : IdentifiedCommandHandler<SetPaidOrderStatusCommand, bool>
    {
        public SetPaidOrderStatusIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<SetPaidOrderStatusCommand, bool>> logger
        ) : base(mediator, requestManager, logger) { }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}