namespace Ordering.Api.Application.Validators
{
    public class GetOrdersByUserQueryValidator : AbstractValidator<GetOrdersByUserQuery>
    {
        public GetOrdersByUserQueryValidator()
        {
            RuleFor(q => q.UserId).NotEmpty().Must(userId => userId != Guid.Empty).WithMessage("UserId is not valid");
        }
    }
}