namespace Ordering.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of repository for <see cref="Buyer" /> entity 
    /// that uses EntityFrameworkCore for operations.
    /// </summary>
    public class BuyerRepository : IBuyerRepository
    {
        private readonly OrderingContext context;

        /// <summary>
        /// Creates new instance of <see cref="BuyerRepository" /> with given <paramref name="context" />.
        /// </summary>
        /// <param name="context">Current <see cref="OrderingContext" />.</param>
        /// <exception cref="ArgumentNullException" />
        public BuyerRepository(OrderingContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => context;

        public Buyer? GetById(int id)
        {
            return context.Buyers
                .Include(b => b.PaymentMethods)
                .Where(b => b.Id == id)
                .FirstOrDefault();
        }

        public async Task<Buyer?> GetByIdAsync(int id)
        {
            return await context.Buyers
                .Include(b => b.PaymentMethods)
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
        }

        public Buyer? GetByIdentity(Guid identityId)
        {
            return context.Buyers
                .Include(b => b.PaymentMethods)
                .Where(b => b.IdentityId == identityId)
                .FirstOrDefault();
        }

        public async Task<Buyer?> GetByIdentityAsync(Guid identityId)
        {
            return await context.Buyers
                .Include(b => b.PaymentMethods)
                .Where(b => b.IdentityId == identityId)
                .FirstOrDefaultAsync();
        }

        public Buyer Add(Buyer buyer)
        {
            if (buyer.IsTransient())
            {
                return context.Buyers.Add(buyer).Entity;
            }

            return buyer;
        }

        public Buyer Update(Buyer buyer)
        {
            return context.Buyers.Update(buyer).Entity;
        }
    }
}