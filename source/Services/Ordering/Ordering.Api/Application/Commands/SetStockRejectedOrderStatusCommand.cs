namespace Ordering.Api.Application.Commands
{
    [DataContract]
    public class SetStockRejectedOrderStatusCommand : IRequest<bool>
    {
        public SetStockRejectedOrderStatusCommand(int orderId, List<int> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }

        [DataMember]
        public int OrderId { get; private set; }

        [DataMember]
        public List<int> OrderStockItems { get; private set; }
    }
}