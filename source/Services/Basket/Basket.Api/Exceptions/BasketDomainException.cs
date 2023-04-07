namespace Basket.Api.Exceptions
{
    public class BasketDomainException : Exception
    {
        public BasketDomainException() : base() { }

        public BasketDomainException(string message) : base(message) { }

        public BasketDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}