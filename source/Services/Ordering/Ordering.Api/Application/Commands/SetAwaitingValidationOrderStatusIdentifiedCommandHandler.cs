namespace Ordering.Api.Application.Commands
{
    public class SetAwaitingValidationOrderStatusIdentifiedCommandHandler : IdentifiedCommandHandler<SetAwaitingValidationOrderStatusCommand, bool>
    {
        public SetAwaitingValidationOrderStatusIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<SetAwaitingValidationOrderStatusCommand, bool>> logger
        ) : base(mediator, requestManager, logger) { }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}