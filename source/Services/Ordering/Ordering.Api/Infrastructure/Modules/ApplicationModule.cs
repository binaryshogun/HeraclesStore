namespace Ordering.Api.Infrastructure.Modules
{
    public class ApplicationModule : Module
    {
        private readonly string connectionString;

        public ApplicationModule(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Register request manager
            builder.RegisterType<RequestManager>()
                .As<IRequestManager>()
                .InstancePerLifetimeScope();

            // Register order repository
            builder.RegisterType<OrderRepository>()
                .As<IOrderRepository>()
                .InstancePerLifetimeScope();

            // Register buyer repository
            builder.RegisterType<BuyerRepository>()
                .As<IBuyerRepository>()
                .InstancePerLifetimeScope();

            // Register sql connection factory
            builder.Register(c => new SqlConnectionFactory(connectionString))
                .As<ISqlConnectionFactory>()
                .InstancePerLifetimeScope();
        }
    }
}