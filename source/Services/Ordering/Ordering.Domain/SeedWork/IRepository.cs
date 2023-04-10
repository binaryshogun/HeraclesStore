namespace Ordering.Domain.SeedWork
{
    public interface IRepository<T> where T : Entity
    {
        IUnitOfWork UnitOfWork { get; }
    }
}