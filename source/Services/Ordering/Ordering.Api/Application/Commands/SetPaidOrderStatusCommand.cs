namespace Ordering.Api.Application.Commands
{
    [DataContract]
    public class SetPaidOrderStatusCommand : IRequest<bool>
    {
        public SetPaidOrderStatusCommand(int orderId)
        {
            OrderId = orderId;
        }

        [DataMember]
        public int OrderId { get; private set; }
    }
}