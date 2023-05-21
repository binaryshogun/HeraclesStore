namespace Ordering.Api.Application.Commands
{
    [DataContract]
    public class CreateOrderCommand : IRequest<bool>
    {
        [DataMember]
        private readonly List<OrderItemDto> orderItems;

        public CreateOrderCommand()
        {
            orderItems = new List<OrderItemDto>();
        }

        public CreateOrderCommand(List<BasketItem> basketItems, Guid userId, string username,
            string city, string street, string state, string country, string zipCode,
            string cardNumber, string cardHolderName, DateTime cardExpiration,
            string cardSecurityNumber, int cardTypeId) : this()
        {
            orderItems = basketItems.Select(bi =>
                new OrderItemDto()
                {
                    ProductId = bi.ProductId,
                    ProductName = bi.ProductName,
                    UnitPrice = bi.UnitPrice,
                    Units = bi.Quantity,
                    PictureUrl = bi.PictureUrl
                }).ToList();
            UserId = userId;
            Username = username;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipCode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
        }

        [DataMember]
        public IEnumerable<OrderItemDto> OrderItems => orderItems;

        [DataMember]
        public Guid UserId { get; private set; }

        [DataMember]
        public string? Username { get; private set; }

        [DataMember]
        public string? City { get; private set; }

        [DataMember]
        public string? Street { get; private set; }

        [DataMember]
        public string? State { get; private set; }

        [DataMember]
        public string? Country { get; private set; }

        [DataMember]
        public string? ZipCode { get; private set; }

        [DataMember]
        public string? CardNumber { get; private set; }

        [DataMember]
        public string? CardHolderName { get; private set; }

        [DataMember]
        public DateTime CardExpiration { get; private set; }

        [DataMember]
        public string? CardSecurityNumber { get; private set; }

        [DataMember]
        public int CardTypeId { get; private set; }
    }
}