using Ordering.Domain.Events;

namespace Ordering.UnitTests.Domain
{
    public class OrderAggregateTests
    {
        [Fact]
        public void CreateOrderItem_OrderShouldBeCreated()
        {
            // Given   
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 5;

            // When 
            var fakeOrderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);

            // Then
            Assert.NotNull(fakeOrderItem);
        }

        [Fact]
        public void CreateOrder_InvalidNumberOfUnits_ShouldThrowDomainException()
        {
            // Given   
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = -1;

            // Then
            Assert.Throws<OrderingDomainException>(() => new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units));
        }

        [Fact]
        public void CreateOrder_TotalOfOrderItemLowerThanDiscount_ShouldThrowDomainException()
        {
            // Given   
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 1;

            // Then
            Assert.Throws<OrderingDomainException>(() => new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units));
        }

        [Fact]
        public void CreateOrder_InvalidDiscountSetting_ShouldThrowDomainException()
        {
            // Given   
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 5;

            // When 
            var fakeOrderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);

            // Then
            Assert.Throws<OrderingDomainException>(() => fakeOrderItem.Discount = -1);
        }

        [Fact]
        public void CreateOrder_InvalidUnitsSetting_ShouldThrowDomainException()
        {
            // Given   
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 5;

            // When 
            var fakeOrderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);

            // Then
            Assert.Throws<OrderingDomainException>(() => fakeOrderItem.AddUnits(-1));
        }

        [Fact]
        public void AddItemsToOrder_TwoSameItems_UnitsShouldAddUp()
        {
            var address = new AddressBuilder().Build();
            var order = new OrderBuilder(address)
                .AddItem(1, "cup", 10.0m, 0, string.Empty)
                .AddItem(1, "cup", 10.0m, 0, string.Empty)
                .Build();

            Assert.Equal(20.0m, order.Total);
        }

        [Fact]
        public void CreateOrder_ShouldAddDomainEvent()
        {
            // Given
            var street = "fakeStreet";
            var city = "FakeCity";
            var state = "fakeState";
            var country = "fakeCountry";
            var zipcode = "FakeZipCode";
            var cardTypeId = 5;
            var cardNumber = "12";
            var cardSecurityNumber = "123";
            var cardHolderName = "FakeName";
            var cardExpiration = DateTime.Now.AddYears(1);
            var expectedResult = 1;

            // When 
            var fakeOrder = new Order(Guid.NewGuid(), "fakeName", new Address(street, city, state, country, zipcode), cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);

            // Then
            Assert.NotNull(fakeOrder.DomainEvents);
            Assert.Equal(expectedResult, fakeOrder.DomainEvents.Count);
        }

        [Fact]
        public void AddDomainEvent_AddedEventShouldBeInEventList()
        {
            // Given  
            var street = "fakeStreet";
            var city = "FakeCity";
            var state = "fakeState";
            var country = "fakeCountry";
            var zipcode = "FakeZipCode";
            var cardTypeId = 5;
            var cardNumber = "12";
            var cardSecurityNumber = "123";
            var cardHolderName = "FakeName";
            var cardExpiration = DateTime.Now.AddYears(1);
            var expectedResult = 2;

            // When 
            var fakeOrder = new Order(Guid.NewGuid(), "fakeName", new Address(street, city, state, country, zipcode), cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
            fakeOrder.AddDomainEvent(new OrderStartedDomainEvent(fakeOrder, Guid.NewGuid(), "fakeName", cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration));

            // Then
            Assert.NotNull(fakeOrder.DomainEvents);
            Assert.Equal(expectedResult, fakeOrder.DomainEvents.Count);
        }

        [Fact]
        public void RemoveDomainEvent_EventShouldBe()
        {
            // Given   
            var street = "fakeStreet";
            var city = "FakeCity";
            var state = "fakeState";
            var country = "fakeCountry";
            var zipcode = "FakeZipCode";
            var cardTypeId = 5;
            var cardNumber = "12";
            var cardSecurityNumber = "123";
            var cardHolderName = "FakeName";
            var cardExpiration = DateTime.Now.AddYears(1);
            var fakeOrder = new Order(Guid.NewGuid(), "fakeName", new Address(street, city, state, country, zipcode), cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
            var fakeEvent = new OrderStartedDomainEvent(fakeOrder, Guid.NewGuid(), "fakeName", cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
            var expectedResult = 1;

            // When         
            fakeOrder.AddDomainEvent(fakeEvent);
            fakeOrder.RemoveDomainEvent(fakeEvent);

            // Then
            Assert.NotNull(fakeOrder.DomainEvents);
            Assert.Equal(expectedResult, fakeOrder.DomainEvents.Count);
        }
    }
}