namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// Abstract class that represents enumeration.
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        /// <summary>
        /// Creates new instance of <see cref="Enumeration" /> with given <paramref name="id" /> and <paramref name="name" />.
        /// </summary>
        /// <param name="id">Enumeration element identifier.</param>
        /// <param name="name">Enumeration element name.</param>
        /// <returns>New instance of <see cref="Enumeration" />.</returns>
        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public int Id { get; private set; }
        public string Name { get; private set; }

        /// <summary>
        /// Gets all elements of <see cref="Enumeration" /> type.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="Enumeration" />.</typeparam>
        /// <returns>All elements of <see cref="Enumeration" /> type.</returns>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                        .Select(f => f.GetValue(null))
                        .Cast<T>();
        }

        /// <inheritdoc cref="IComparable.CompareTo(object?)" />
        public int CompareTo(object? other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var enumeration = other as Enumeration;

            if (enumeration is null)
            {
                throw new ArgumentException("Object is not Enumeration", nameof(other));
            }

            return Id.CompareTo(enumeration.Id);
        }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString() => Name;

        /// <inheritdoc cref="object.Equals(object?)" />
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        /// <inheritdoc cref="object.GetHashCode" />
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ 31;
        }

        /// <summary>
        /// Gets absolute difference between two <see cref="Enumeration" /> values by calculating difference between two <see cref="Enumeration.Id" />.
        /// </summary>
        /// <param name="firstValue">First <see cref="Enumeration" /> element.</param>
        /// <param name="secondValue">Second <see cref="Enumeration" /> element.</param>
        /// <returns>Absolute difference between two <see cref="Enumeration.Id" /> properties.</returns>
        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        /// <summary>
        /// Gets enumeration element of type <typeparamref name="T" /> by <paramref name="value" /> of identifier.
        /// </summary>
        /// <param name="value">Enumeration element identifier.</param>
        /// <typeparam name="T">Type of enumeration element.</typeparam>
        /// <returns>Enumeration element of type <typeparamref name="T" /> if exists; otherwise - <see langword="null" />.</returns>
        public static T? FromValue<T>(int value) where T : Enumeration
        {
            return Parse<T>(item => item.Id == value);
        }

        /// <summary>
        /// Gets enumeration element of type <typeparamref name="T" /> by <paramref name="displayName" />.
        /// </summary>
        /// <param name="displayName">Enumeration element name.</param>
        /// <typeparam name="T">Type of enumeration element.</typeparam>
        /// <returns>Enumeration element of type <typeparamref name="T" /> if exists; otherwise - <see langword="null" />.</returns>
        public static T? FromDisplayName<T>(string displayName) where T : Enumeration
        {
            return Parse<T>(item => item.Name == displayName);
        }

        /// <summary>
        /// Parses enumeration element of type <typeparamref name="T" /> which satisfies <paramref name="predicate" /> condition.
        /// </summary>
        /// <param name="predicate">Logical expression for selecting an element.</param>
        /// <typeparam name="T">Type of enumeration element.</typeparam>
        /// <returns>Enumeration element of type <typeparamref name="T" /> if exists; otherwise - <see langword="null" />.</returns>
        private static T? Parse<T>(Func<T, bool> predicate) where T : Enumeration
        {
            return GetAll<T>().FirstOrDefault(predicate);
        }
    }
}