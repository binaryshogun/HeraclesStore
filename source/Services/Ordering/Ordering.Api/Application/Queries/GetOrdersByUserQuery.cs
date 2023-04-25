namespace Ordering.Api.Application.Queries
{
    public class GetOrdersByUserQuery : IRequest<IEnumerable<OrderSummary>>
    {
        public GetOrdersByUserQuery(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; init; }
    }
}