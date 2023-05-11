namespace Ordering.UnitTests.Domain.Models.Builders
{
    public class AddressBuilder
    {
        public Address Build()
        {
            return new Address("street", "city", "state", "country", "zipcode");
        }
    }
}