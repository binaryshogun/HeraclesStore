namespace Ordering.Api.Infrastructure.Modules
{
    public class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register validators
            builder.RegisterAssemblyTypes(typeof(GetOrderQueryValidator).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            // Register command handlers as integration event handlers
            builder.RegisterAssemblyTypes(typeof(CreateOrderCommandHandler).Assembly).AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            // Register all the command handlers
            builder.RegisterAssemblyTypes(typeof(CreateOrderCommand).Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register all the domain event handlers
            builder.RegisterAssemblyTypes(typeof(BuyerPaymentMethodVerifiedDomainEventHandler).Assembly).AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register behaviors
            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}