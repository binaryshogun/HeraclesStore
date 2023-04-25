namespace Ordering.Api.Application.Validators
{
    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(c => c.OrderId).NotEmpty().WithMessage("Order Id is necessary");
        }
    }
}