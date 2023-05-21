namespace Basket.Api.IntegrationEvents.Events
{
    public record UserCheckoutAcceptedIntegrationEvent : IntegrationEvent
    {
        public UserCheckoutAcceptedIntegrationEvent(Guid userId, string userName, string city, string street,
            string state, string country, string zipCode, string cardNumber, string cardHolderName,
            DateTime cardExpiration, string cardSecurityNumber, int cardTypeId, Guid requestId,
            CustomerBasket basket)
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

        public Guid UserId { get; init; }

        public string UserName { get; init; }

        public int OrderNumber { get; init; }

        public string City { get; init; }

        public string Street { get; init; }

        public string State { get; init; }

        public string Country { get; init; }

        public string ZipCode { get; init; }

        public string CardNumber { get; init; }

        public string CardHolderName { get; init; }

        public DateTime CardExpiration { get; init; }

        public string CardSecurityNumber { get; init; }

        public int CardTypeId { get; init; }

        public Guid RequestId { get; init; }

        public CustomerBasket Basket { get; init; }
    }
}