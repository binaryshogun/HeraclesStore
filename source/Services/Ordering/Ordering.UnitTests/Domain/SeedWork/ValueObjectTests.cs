namespace Ordering.UnitTests.Domain.SeedWork
{
    public class ValueObjectTests
    {
        [Theory]
        [MemberData(nameof(EqualValueObjects))]
        public void Equals_EqualValueObjects_ReturnsTrue(ValueObject instanceA, ValueObject instanceB, string reason)
        {
            // Act
            var result = EqualityComparer<ValueObject>.Default.Equals(instanceA, instanceB);

            // Assert
            Assert.True(result, reason);
        }

        [Theory]
        [MemberData(nameof(NonEqualValueObjects))]
        public void Equals_NonEqualValueObjects_ReturnsFalse(ValueObject instanceA, ValueObject instanceB, string reason)
        {
            // Act
            var result = EqualityComparer<ValueObject>.Default.Equals(instanceA, instanceB);

            // Assert
            Assert.False(result, reason);
        }

        private static readonly ValueObject APrettyValueObject = new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"));

        public static readonly TheoryData<ValueObject, ValueObject, string> EqualValueObjects = new()
        {
            {
                null!,
                null!,
                "they should be equal because they are both null"
            },
            {
                APrettyValueObject,
                APrettyValueObject,
                "they should be equal because they are the same object"
            },
            {
                new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3")),
                new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3")),
                "they should be equal because they have equal members"
            },
            {
                new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"), notAnEqualityComponent: "xpto"),
                new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"), notAnEqualityComponent: "xpto2"),
                "they should be equal because all equality components are equal, even though an additional member was set"
            },
            {
                new ValueObjectB(1, "2",  1, 2, 3 ),
                new ValueObjectB(1, "2",  1, 2, 3 ),
                "they should be equal because all equality components are equal, including the 'C' list"
            }
        };

        public static readonly TheoryData<ValueObject, ValueObject, string> NonEqualValueObjects = new()
        {
            {
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                new ValueObjectA(a: 2, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                "they should not be equal because the 'A' member on ValueObjectA is different among them"
            },
            {
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                new ValueObjectA(a: 1, b: null, c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                "they should not be equal because the 'B' member on ValueObjectA is different among them"
            },
            {
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(a: 2, b: "3")),
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(a: 3, b: "3")),
                "they should not be equal because the 'A' member on ValueObjectA's 'D' member is different among them"
            },
            {
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(a: 2, b: "3")),
                new ValueObjectB(a: 1, b: "2"),
                "they should not be equal because they are not of the same type"
            },
            {
                new ValueObjectB(1, "2",  1, 2, 3 ),
                new ValueObjectB(1, "2",  1, 2, 3, 4 ),
                "they should be not be equal because the 'C' list contains one additional value"
            },
            {
                new ValueObjectB(1, "2",  1, 2, 3, 5 ),
                new ValueObjectB(1, "2",  1, 2, 3 ),
                "they should be not be equal because the 'C' list contains one additional value"
            },
            {
                new ValueObjectB(1, "2",  1, 2, 3, 5 ),
                new ValueObjectB(1, "2",  1, 2, 3, 4 ),
                "they should be not be equal because the 'C' lists are not equal"
            }
        };
    }
}