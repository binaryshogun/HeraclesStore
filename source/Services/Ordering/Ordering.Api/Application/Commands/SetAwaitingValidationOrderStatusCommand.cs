namespace Ordering.Api.Application.Commands
{
    [DataContract]
    public class SetAwaitingValidationOrderStatusCommand : IRequest<bool>
    {
        public SetAwaitingValidationOrderStatusCommand(int orderId)
        {
            OrderId = orderId;
        }

        [DataMember]
        public int OrderId { get; }
    }
}