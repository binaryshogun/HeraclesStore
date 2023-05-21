namespace Ordering.Api.Application.Commands
{
    [DataContract]
    public class SetStockConfirmedOrderStatusCommand : IRequest<bool>
    {
        public SetStockConfirmedOrderStatusCommand(int orderId)
        {
            OrderId = orderId;
        }

        [DataMember]
        public int OrderId { get; private set; }
    }
}