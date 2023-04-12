namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// Abstract class representing entity object.
    /// </summary>
    public abstract class Entity
    {
        private int id;
        private List<INotification>? domainEvents;

        public virtual int Id
        {
            get => id;
            protected set => id = value;
        }

        /// <summary>
        /// Read only collection of domain events.
        /// </summary>
        /// <returns><see cref="IReadOnlyCollection{INotification}" /> of domain events.</returns>
        public IReadOnlyCollection<INotification>? DomainEvents => domainEvents?.AsReadOnly();

        /// <summary>
        /// Adds domain event to <see cref="DomainEvents" /> collection.
        /// </summary>
        /// <param name="eventItem">Event that need to be added to collection of domain events.</param>
        public void AddDomainEvent(INotification eventItem)
        {
            domainEvents = domainEvents ?? new List<INotification>();
            domainEvents.Add(eventItem);
        }

        /// <summary>
        /// Removes domain event from <see cref="DomainEvents" /> collection.
        /// </summary>
        /// <param name="eventItem">Event that need to be removed from collection of domain events.</param>
        public void RemoveDomainEvent(INotification eventItem)
        {
            domainEvents?.Remove(eventItem);
        }

        /// <summary>
        /// Clears all domain events.
        /// </summary>
        public void ClearDomainEvents()
        {
            domainEvents?.Clear();
        }

        /// <summary>
        /// Specifies whether the entity is transient or not.
        /// </summary>
        /// <returns><see langword="true" /> if entity is transient; otherwise - <see langword="false" />.</returns>
        public bool IsTransient()
        {
            return this.Id == default(Int32);
        }

        /// <inheritdoc cref="object.Equals(object?)" />
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Entity)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity? item = obj as Entity;

            if (item is null || item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        /// <inheritdoc cref="object.GetHashCode" />
        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                return this.Id.GetHashCode() ^ 31;
            }
            else
                return base.GetHashCode();

        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}