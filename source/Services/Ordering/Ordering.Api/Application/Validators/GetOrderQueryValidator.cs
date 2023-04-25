namespace Ordering.Api.Application.Validators
{
    public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
    {
        public GetOrderQueryValidator()
        {
            RuleFor(q => q.OrderId).NotEmpty().WithMessage("Order Id is necessary");
        }
    }
}