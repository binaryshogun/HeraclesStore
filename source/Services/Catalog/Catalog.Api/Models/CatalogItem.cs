namespace Catalog.Api.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string? PictureFileName { get; set; }
        public string? PictureUri { get; set; }

        public int CatalogTypeId { get; set; }
        public CatalogType? CatalogType { get; set; }

        public int CatalogBrandId { get; set; }
        public CatalogBrand? CatalogBrand { get; set; }

        /// <summary>
        /// Quantity in stock.
        /// </summary>
        /// <value><see cref="int" /></value>
        public int AvailableInStock { get; set; }

        /// <summary>
        /// Available stock at which the product should be reordered.
        /// </summary>
        /// <value></value>
        public int RestockThreshold { get; set; }

        /// <summary>
        /// Maximum number of units that can be in-stock at any time due to physical and logistical constraints.
        /// </summary>
        /// <value><see cref="int" /></value>
        public int MaxStockThreshold { get; set; }

        /// <summary>
        /// Indicates whether the product should be reordered.
        /// </summary>
        /// <value><see cref="bool" /></value>
        public bool OnReorder { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="CatalogItem" />.
        /// </summary>
        public CatalogItem() { }

        /// <summary>
        /// Decrements the quantity of a particular item in stock and ensures the <see cref="RestockThreshold" /> hasn't been breached.
        /// If so, a <see cref="OnReorder" /> property is set to <see langword="true" />.
        /// </summary>
        /// <param name="quantityDesired">A specified quantity to remove from <see cref="AvailableInStock" />. It is invalid to pass 
        /// in a negative number.</param>
        /// <returns>If there is sufficient stock of an item, then the integer returned at the end of this call should be the same as 
        /// <paramref name="quantityDesired" />. In the event that there is not sufficient stock available, the method will remove 
        /// whatever stock is available and return that quantity to the client.</returns>
        public int RemoveStock(int quantityDesired)
        {
            if (AvailableInStock == 0)
            {
                throw new CatalogDomainException($"Empty stock, product item {Name} sold out");
            }

            if (quantityDesired <= 0)
            {
                throw new CatalogDomainException($"Item units desired should be greater than zero");
            }

            int removed = Math.Min(quantityDesired, AvailableInStock);

            AvailableInStock -= removed;

            if (AvailableInStock <= RestockThreshold)
            {
                OnReorder = true;
            }

            return removed;
        }

        /// <summary>
        /// Increments the quantity of a particular item in stock.
        /// </summary>
        /// <param name="quantity">A specified quantity of items.</param>
        /// <returns>Returns the quantity that has been added to stock.</returns>
        public int AddStock(int quantity)
        {
            int original = AvailableInStock;

            if (quantity <= 0)
            {
                throw new CatalogDomainException("Quantity should be greater than zero");
            }

            // Client is trying to add to stock quantity that is greater than could be phisically acommodated in the warehouse
            if ((AvailableInStock + quantity) > MaxStockThreshold)
            {
                AvailableInStock = MaxStockThreshold;
            }
            else
            {
                AvailableInStock += quantity;
            }

            if (AvailableInStock > RestockThreshold)
            {
                OnReorder = false;
            }

            return AvailableInStock - original;
        }

        /// <summary>
        /// Fills the product picture url.
        /// </summary>
        /// <param name="pictureBaseUrl">Base path to folder that contains product picture.</param>
        public void FillPicturePath(string pictureBaseUrl)
        {
            if (this is not null)
            {
                PictureUri = pictureBaseUrl + PictureFileName;
            }
        }
    }
}