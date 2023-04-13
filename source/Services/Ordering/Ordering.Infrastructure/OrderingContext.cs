namespace Ordering.Infrastructure
{
    /// <summary>
    /// Database context for ordering system.
    /// </summary>
    public class OrderingContext : DbContext, IUnitOfWork
    {
        /// <summary>
        /// Default database schema name.
        /// </summary>
        public const string DefaultSchema = "ordering";

        private readonly IMediator? mediator = default!;
        private IDbContextTransaction? currentTransaction = default!;

        /// <summary>
        /// Initializes new instance of <see cref="OrderingContext" /> with given <paramref name="options" />.
        /// </summary>
        /// <param name="options">Database context configuration options.</param>
        public OrderingContext(DbContextOptions<OrderingContext> options)
            : base(options) { }

        /// <summary>
        /// Initializes new instance of <see cref="OrderingContext" /> with given <paramref name="options" /> and <paramref name="mediator" />.
        /// </summary>
        /// <param name="options">Database context configuration options.</param>
        /// <param name="mediator"><see cref="IMediator" /> instance.</param>
        public OrderingContext(DbContextOptions<OrderingContext> options, IMediator mediator) : base(options)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;
        public DbSet<PaymentMethod> Payments { get; set; } = default!;
        public DbSet<Buyer> Buyers { get; set; } = default!;
        public DbSet<CardType> CardTypes { get; set; } = default!;
        public DbSet<OrderStatus> OrderStatus { get; set; } = default!;

        /// <summary>
        /// Current database transaction.
        /// </summary>
        public IDbContextTransaction? CurrentTransaction => currentTransaction;
        /// <summary>
        /// Specifies if <see cref="OrderingContext" /> has active database transaction.
        /// </summary>
        public bool HasActiveTransaction => currentTransaction is not null;

        /// <summary>
        /// Configures database model for <see cref="OrderingContext" />.
        /// </summary>
        /// <param name="modelBuilder">Database model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());

        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await (mediator?.DispatchDomainEventsAsync(this) ?? Task.CompletedTask);

            return await base.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <summary>
        /// Asynchronously begins <see cref="CurrentTransaction" /> execution.
        /// </summary>
        /// <returns><see cref="Task" /> that represents asynchronous operation 
        /// and contains <see cref="IDbContextTransaction" /> value if transaction
        /// was successfully started, overwise - <see langword="null" />.</returns>
        public async Task<IDbContextTransaction?> BeginTransactionAsync()
        {
            if (currentTransaction is not null)
            {
                return null;
            }

            currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return currentTransaction;
        }

        /// <summary>
        /// Asynchronously commits given <paramref name="transaction" />.
        /// </summary>
        /// <param name="transaction">Database transaction.</param>
        /// <returns><see cref="Task" /> that represents asynchronous operation.</returns>
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction is null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (transaction != currentTransaction)
            {
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
            }

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                RollbackTransaction();

                throw ex;
            }
            finally
            {
                ClearTransaction();
            }
        }

        /// <summary>
        /// Rolling back transaction execution.
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                currentTransaction?.Rollback();
            }
            finally
            {
                ClearTransaction();
            }
        }

        /// <summary>
        /// Clears <see cref="CurrentTransaction" />.
        /// </summary>
        private void ClearTransaction()
        {
            if (currentTransaction is not null)
            {
                currentTransaction.Dispose();
                currentTransaction = null;
            }
        }
    }
}