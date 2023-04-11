namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// Abstract class representing object containing composite value.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Compares two <see cref="ValueObject" /> by reference.
        /// </summary>
        /// <param name="left">First <see cref="ValueObject" /> value.</param>
        /// <param name="right">Second <see cref="ValueObject" /> value.</param>
        /// <returns><see langword="true" /> if values are equal by reference; otherwise - <see langword="false" />.</returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="ValueObject" /> by reference.
        /// </summary>
        /// <param name="left">First <see cref="ValueObject" /> value.</param>
        /// <param name="right">Second <see cref="ValueObject" /> value.</param>
        /// <returns><see langword="true" /> if values are not equal by reference; otherwise - <see langword="false" />.</returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        /// <summary>
        /// Gets all stored values from value object's properties.
        /// </summary>
        /// <returns><see cref="IEnumerable{object?}" /> containing all value object's properties values.</returns>
        protected abstract IEnumerable<object?> GetEqualityComponents();

        /// <inheritdoc cref="object.Equals(object?)" />
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not ValueObject)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            var item = obj as ValueObject;

            if (item is null)
            {
                return false;
            }

            return this.GetEqualityComponents().SequenceEqual(item.GetEqualityComponents());
        }

        /// <inheritdoc cref="object.GetHashCode" />
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// Gets the copy of <see cref="ValueObject" />.
        /// </summary>
        /// <returns>Copy of <see cref="ValueObject" /> instance.</returns>
        public ValueObject GetCopy()
        {
            return (this.MemberwiseClone() as ValueObject)!;
        }

        public static bool operator ==(ValueObject left, ValueObject right) => EqualOperator(left, right);
        public static bool operator !=(ValueObject left, ValueObject right) => NotEqualOperator(left, right);
    }
}