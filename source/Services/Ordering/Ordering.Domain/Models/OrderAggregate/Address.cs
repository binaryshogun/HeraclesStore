namespace Ordering.Domain.Models.OrderAggregate
{
    /// <summary>
    /// Value object that contains address information.
    /// </summary>
    public class Address : ValueObject
    {
        public string? Street { get; private set; }
        public string? City { get; private set; }
        public string? State { get; private set; }
        public string? Country { get; private set; }
        public string? ZipCode { get; private set; }

        public Address() { }

        /// <summary>
        /// Creates new <see cref="Address" /> instance with given <paramref name="street" />, <paramref name="city" />, 
        /// <paramref name="state" />, <paramref name="country" /> and <paramref name="zipCode" />.
        /// </summary>
        /// <param name="street"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <param name="zipCode"></param>
        public Address(string street, string city, string state, string country, string zipCode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
}