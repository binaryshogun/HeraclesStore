namespace Ordering.Domain.Models.BuyerAggregate
{
    public class PaymentMethod : Entity
    {
        private string alias = default!;

        private string cardNumber = default!;
        private string securityNumber = default!;
        private string cardHolderName = default!;
        private DateTime expiration;

        private int cardTypeId;
        public CardType? CardType { get; private set; }

        protected PaymentMethod() { }

        public PaymentMethod(string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration, int cardTypeId)
        {
            this.alias = alias;

            this.cardNumber = string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException("Invalid card number");
            this.securityNumber = string.IsNullOrWhiteSpace(securityNumber) ? cardNumber : throw new OrderingDomainException("Invalid security number");
            this.cardHolderName = string.IsNullOrWhiteSpace(cardHolderName) ? cardNumber : throw new OrderingDomainException("Invalid card holder name");
            this.expiration = expiration >= DateTime.UtcNow ? expiration : throw new OrderingDomainException("Invalid expiration date");

            this.cardTypeId = cardTypeId;
        }

        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return this.cardTypeId == cardTypeId && this.cardNumber == cardNumber && this.expiration == expiration;
        }
    }
}