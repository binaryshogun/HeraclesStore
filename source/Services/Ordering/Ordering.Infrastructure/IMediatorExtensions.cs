namespace Ordering.Infrastructure
{
    /// <summary>
    /// Class with extensions methods for <see cref="IMediator" />.
    /// </summary>
    public static class IMediatorExtensions
    {
        /// <summary>
        /// Asynchronously publishes all domain events from <see cref="Entity.DomainEvents" /> within all <see cref="Entity" /> exemplares from <see cref="OrderingContext" />.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator" /> instance.</param>
        /// <param name="context"><see cref="OrderingContext" /> instance.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation.</returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderingContext context)
        {
            var domainEntities = context.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents is not null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();

            domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (INotification domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}