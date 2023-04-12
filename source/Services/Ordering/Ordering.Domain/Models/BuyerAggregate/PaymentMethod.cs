namespace Ordering.Domain.Models.BuyerAggregate
{
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// Creates new instanse of <see cref="PaymentMethod" />.
        /// </summary>
        /// <param name="alias">Payment method name.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <param name="securityNumber">Card security number.</param>
        /// <param name="cardHolderName">Card holder name.</param>
        /// <param name="expiration">Card expiration date.</param>
        /// <param name="cardTypeId">Card type identifier.</param>
        public PaymentMethod(string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration, int cardTypeId)
        {
            this.alias = alias;

            this.cardNumber = string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException("Invalid card number");
            this.securityNumber = string.IsNullOrWhiteSpace(securityNumber) ? cardNumber : throw new OrderingDomainException("Invalid security number");
            this.cardHolderName = string.IsNullOrWhiteSpace(cardHolderName) ? cardNumber : throw new OrderingDomainException("Invalid card holder name");
            this.expiration = expiration >= DateTime.UtcNow ? expiration : throw new OrderingDomainException("Invalid expiration date");

            this.cardTypeId = cardTypeId;
        }

        /// <summary>
        /// Compares payment methods by card info: <paramref name="cardTypeId" />, <paramref name="cardNumber" /> and <paramref name="expiration" />.
        /// </summary>
        /// <param name="cardTypeId">Card type identifier.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <param name="expiration">Card expiration date.</param>
        /// <returns><see langword="true" /> if methods are equal; otherwise - <see langword="false" />.</returns>
        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return this.cardTypeId == cardTypeId && this.cardNumber == cardNumber && this.expiration == expiration;
        }
    }
}