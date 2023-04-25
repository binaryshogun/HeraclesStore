namespace Ordering.Api.Application.Commands
{
    public class ShipOrderCommand : IRequest<bool>
    {
        public ShipOrderCommand(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; init; }
    }
}