namespace Ordering.Api.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            this.validators = validators;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var type = request.GetType();

            logger.LogInformation("[Ordering] ---> Validating request {RequestType}", type);

            var failures = validators
                .Select(v => v.Validate(request))
                .SelectMany(r => r.Errors)
                .Where(e => e is not null)
                .ToList();

            if (failures.Any())
            {
                logger.LogWarning("[Ordering] ---> Errors occurred while validating {RequestType} request: {@Request}. List of errors: {@Errors}",
                    type, request, failures);

                throw new OrderingDomainException($"Validation errors for request of type {typeof(TRequest).Name}",
                    new FluentValidation.ValidationException("ValidationException", failures));
            }

            return await next();
        }
    }
}