namespace Ordering.Api.Application.Events.Integration
{
    public record UserCheckoutAcceptedIntegrationEvent : IntegrationEvent
    {
        public UserCheckoutAcceptedIntegrationEvent(
            Guid userId, string userName, string city, string street, string state, string country, string zipCode,
            string cardNumber, string cardHolderName, DateTime cardExpiration, string cardSecurityNumber,
            int cardTypeId, Guid requestId, CustomerBasket basket)
        {
            UserId = userId;
            UserName = userName;

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

            Basket = basket;
            RequestId = requestId;
        }

        public Guid UserId { get; }

        public string UserName { get; }

        public string City { get; }

        public string Street { get; }

        public string State { get; }

        public string Country { get; }

        public string ZipCode { get; }

        public string CardNumber { get; }

        public string CardHolderName { get; }

        public DateTime CardExpiration { get; }

        public string CardSecurityNumber { get; }

        public int CardTypeId { get; }

        public Guid RequestId { get; }

        public CustomerBasket Basket { get; }
    }
}