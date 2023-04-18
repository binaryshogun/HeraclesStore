namespace Ordering.Api.Application.Commands
{
    public class CancelOrderCommand : IRequest<bool>
    {
        public CancelOrderCommand(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; init; }
    }
}