namespace Ordering.Api.Application.Queries
{
    public class GetOrderQuery : IRequest<OrderDetails>
    {
        public GetOrderQuery(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; init; }
    }
}