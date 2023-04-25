namespace Ordering.Domain.Models.BuyerAggregate
{
    /// <summary>
    /// <see cref="Enumeration" /> containing available card types.
    /// </summary>
    public class CardType : Enumeration
    {
        public static CardType Visa = new(1, nameof(Visa));
        public static CardType MasterCard = new(2, nameof(MasterCard));

        /// <summary>
        /// Creates new instance of <see cref="CardType" /> with given <paramref name="id" /> and <paramref name="name" />.
        /// </summary>
        /// <param name="id">Enum element identifier.</param>
        /// <param name="name">Enum element name.</param>
        public CardType(int id, string name)
            : base(id, name) { }
    }
}