namespace Ordering.Api.Application.Validators
{
    public class ShipOrderCommandValidator : AbstractValidator<ShipOrderCommand>
    {
        public ShipOrderCommandValidator()
        {
            RuleFor(c => c.OrderId).NotEmpty().WithMessage("Order Id is necessary");
        }
    }
}