namespace Ordering.Api.Application.Validators
{
    public class IdentifiedCommandValidator : AbstractValidator<IdentifiedCommand<CreateOrderCommand, bool>>
    {
        public IdentifiedCommandValidator(ILogger<IdentifiedCommandValidator> logger)
        {
            RuleFor(command => command.CommandId).NotEmpty();
        }
    }
}