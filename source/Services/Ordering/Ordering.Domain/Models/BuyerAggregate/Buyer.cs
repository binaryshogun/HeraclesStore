namespace Ordering.Domain.Models.BuyerAggregate
{
    /// <summary>
    /// Represents buyer entity.
    /// </summary>
    public class Buyer : Entity
    {
        private List<PaymentMethod> paymentMethods;

        /// <summary>
        /// Initializes new <see cref="Buyer" /> object instance.
        /// </summary>
        protected Buyer()
        {
            paymentMethods = new List<PaymentMethod>();
        }

        /// <summary>
        /// Initializes new <see cref="Buyer" /> object instance with given <paramref name="identityId" /> and <paramref name="name" />.
        /// </summary>
        /// <param name="identityId">User identity identifier.</param>
        /// <param name="name">Username.</param>
        /// <returns>New instance of <see cref="Buyer" />.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Buyer(Guid identityId, string name) : this()
        {
            IdentityId = identityId != Guid.Empty ? identityId : throw new ArgumentException("Invalid identity Id value", nameof(identityId));
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
        }

        public Guid IdentityId { get; private set; }
        public string Name { get; private set; } = default!;

        public IReadOnlyCollection<PaymentMethod> PaymentMethods => paymentMethods.AsReadOnly();

        /// <summary>
        /// Verifies or adds buyer's payment method.
        /// </summary>
        /// <param name="alias">Payment method name.</param>
        /// <param name="cardNumber">Card number.</param>
        /// <param name="securityNumber">Card security number.</param>
        /// <param name="cardHolderName">Card holder name.</param>
        /// <param name="expiration">Card expiration date.</param>
        /// <param name="cardTypeId">Card type identifier.</param>
        /// <param name="orderId">Order identifier.</param>
        /// <returns>Verified <see cref="PaymentMethod" />.</returns>
        public PaymentMethod VerifyOrAddPaymentMethod(string alias, string cardNumber, string securityNumber,
            string cardHolderName, DateTime expiration,
            int cardTypeId, int orderId)
        {
            var existingPayment = paymentMethods
                .SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

            if (existingPayment is not null)
            {
                AddDomainEvent(new BuyerPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingPayment;
            }

            var payment = new PaymentMethod(alias, cardNumber, securityNumber, cardHolderName, expiration, cardTypeId);

            paymentMethods.Add(payment);

            AddDomainEvent(new BuyerPaymentMethodVerifiedDomainEvent(this, payment, orderId));

            return payment;
        }
    }
}