namespace Ordering.UnitTests.Domain
{
    public class BuyerAggregateTests
    {
        [Fact]
        public void CreateBuyer_BuyerShouldBeCreated()
        {
            // Given    
            var identity = Guid.NewGuid();
            var name = "fakeUser";

            // When 
            var buyer = new Buyer(identity, name);

            // Then
            Assert.NotNull(buyer);
        }

        [Fact]
        public void CreateBuyer_InvalidIdentity_ShouldThrowArgumentException()
        {
            // Given    
            var identity = Guid.Empty;
            var name = "fakeUser";

            // When - Assert
            Assert.Throws<ArgumentException>(() => new Buyer(identity, name));
        }

        [Fact]
        public void AddPaymentMethod_PaymentMethodShouldBeAdded()
        {
            // Given    
            var cardTypeId = 1;
            var alias = "fakeAlias";
            var cardNumber = "124";
            var securityNumber = "1234";
            var cardHolderName = "FakeHolderNAme";
            var expiration = DateTime.Now.AddYears(1);
            var orderId = 1;
            var name = "fakeUser";
            var identity = Guid.NewGuid();
            var buyer = new Buyer(identity, name);

            // When
            var result = buyer.VerifyOrAddPaymentMethod(alias, cardNumber, securityNumber, cardHolderName, expiration, cardTypeId, orderId);

            // Then
            Assert.NotNull(result);
        }

        [Fact]
        public void CreatePaymentMethod_PaymentMethodShouldBeCreated()
        {
            // Given    
            var cardTypeId = 1;
            var alias = "fakeAlias";
            var cardNumber = "124";
            var securityNumber = "1234";
            var cardHolderName = "FakeHolderNAme";
            var expiration = DateTime.Now.AddYears(1);
            var paymentMethod = new PaymentMethod(alias, cardNumber, securityNumber, cardHolderName, expiration, cardTypeId);

            // When
            var result = new PaymentMethod(alias, cardNumber, securityNumber, cardHolderName, expiration, cardTypeId);

            // Then
            Assert.NotNull(result);
        }

        [Fact]
        public void CreatePaymentMethod_WrongCardExpiration_ShouldThrowDomainException()
        {
            // Given    
            var cardTypeId = 1;
            var alias = "fakeAlias";
            var cardNumber = "124";
            var securityNumber = "1234";
            var cardHolderName = "FakeHolderNAme";
            var expiration = DateTime.Now.AddYears(-1);

            // When - Assert
            Assert.Throws<OrderingDomainException>(() => new PaymentMethod(alias, cardNumber, securityNumber, cardHolderName, expiration, cardTypeId));
        }

        [Fact]
        public void PaymentMethod_IsEqualTo_ShouldReturnTrue()
        {
            // Given    
            var cardTypeId = 1;
            var alias = "fakeAlias";
            var cardNumber = "124";
            var securityNumber = "1234";
            var cardHolderName = "FakeHolderNAme";
            var expiration = DateTime.Now.AddYears(1);

            // When
            var paymentMethod = new PaymentMethod(alias, cardNumber, securityNumber, cardHolderName, expiration, cardTypeId);
            var result = paymentMethod.IsEqualTo(cardTypeId, cardNumber, expiration);

            // Then
            Assert.True(result);
        }

        [Fact]
        public void AddNewPaymentMethod_ShouldRaiseNewEvent()
        {
            // Given    
            var alias = "fakeAlias";
            var orderId = 1;
            var cardTypeId = 5;
            var cardNumber = "12";
            var cardSecurityNumber = "123";
            var cardHolderName = "FakeName";
            var cardExpiration = DateTime.Now.AddYears(1);
            var expectedResult = 1;
            var name = "fakeUser";

            // When 
            var buyer = new Buyer(Guid.NewGuid(), name);
            buyer.VerifyOrAddPaymentMethod(alias, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration, orderId, cardTypeId);

            // Then
            Assert.NotNull(buyer.DomainEvents);
            Assert.Equal(buyer.DomainEvents.Count, expectedResult);
        }
    }
}