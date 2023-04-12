namespace Ordering.Domain.Models.OrderAggregate
{
    /// <summary>
    /// Represents order item entity.
    /// </summary>
    public class OrderItem : Entity
    {
        private string productName = default!;
        private string? pictureUrl;
        private decimal unitPrice;
        private decimal discount;
        private int units;

        protected OrderItem() { }

        /// <summary>
        /// Creates new instance of <see cref="OrderItem" /> with given <paramref name="productId" />, 
        /// <paramref name="productName" />, <paramref name="unitPrice" />, <paramref name="discount" />,
        /// <paramref name="pictureUrl" /> and <paramref name="units" />.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="productName">Product name.</param>
        /// <param name="unitPrice">Price per unit.</param>
        /// <param name="discount">Discount value.</param>
        /// <param name="pictureUrl">Url of product picture.</param>
        /// <param name="units">Number of units.</param>
        public OrderItem(int productId, string productName, decimal unitPrice, decimal discount, string? pictureUrl = null, int units = 1)
        {
            CheckDiscountValue(unitPrice, discount, units);

            ProductId = productId;
            ProductName = productName;
            Units = units;
            UnitPrice = unitPrice;
            Discount = discount;
            PictureUrl = pictureUrl;
        }

        public int ProductId { get; private set; }

        public string ProductName
        {
            get => productName;
            private set => productName = !string.IsNullOrEmpty(value) ? value : throw new OrderingDomainException("Product name cannot be null");
        }

        public decimal Discount
        {
            get => discount;
            set
            {
                if (value < 0)
                {
                    throw new OrderingDomainException("Discount value cannot be less than zero");
                }

                discount = value;
            }
        }

        public string? PictureUrl
        {
            get => pictureUrl;
            private set => pictureUrl = value;
        }

        public int Units
        {
            get => units;
            private set => units = value > 0 ? value : throw new OrderingDomainException("Number of units cannot be less or equal to zero");
        }

        public decimal UnitPrice
        {
            get => unitPrice;
            private set => unitPrice = value;
        }

        /// <summary>
        /// Adds the given number of <paramref name="units" /> to order item.
        /// </summary>
        /// <param name="units">Number of units.</param>
        public void AddUnits(int units)
        {
            if (units < 0)
            {
                throw new OrderingDomainException("Units value cannot be less than zero");
            }

            this.units += units;
        }

        /// <summary>
        /// Checks discount value for order item.
        /// </summary>
        /// <param name="unitPrice">Price per unit.</param>
        /// <param name="discount">Full discount value.</param>
        /// <param name="units">Number of units.</param>
        /// <exception cref="OrderingDomainException" />
        private static void CheckDiscountValue(decimal unitPrice, decimal discount, int units)
        {
            if ((unitPrice * units) < discount)
            {
                throw new OrderingDomainException("The total of order item is lower than applied discount");
            }
        }
    }
}