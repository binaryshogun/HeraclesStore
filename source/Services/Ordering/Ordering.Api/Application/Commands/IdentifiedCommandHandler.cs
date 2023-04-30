namespace Ordering.Api.Application.Commands
{
    public class IdentifiedCommandHandler<T, R> : IRequestHandler<IdentifiedCommand<T, R>, R>
        where T : IRequest<R>
    {
        private readonly IMediator mediator;
        private readonly IRequestManager requestManager;
        private readonly ILogger<IdentifiedCommandHandler<T, R>> logger;

        public IdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<T, R>> logger)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<R> Handle(IdentifiedCommand<T, R> request, CancellationToken cancellationToken)
        {
            var requestAlreadyExists = await requestManager.ExistsAsync(request.CommandId);

            if (requestAlreadyExists)
            {
                return CreateDefaultResult();
            }

            await requestManager.CreateRequestForCommandAsync<T>(request.CommandId);

            try
            {
                logger.LogInformation("[Ordering] ---> Sending command {CommandName} ({@Command})",
                    typeof(T).Name, request.Command);

                var response = await mediator.Send(request.Command, cancellationToken);

                logger.LogInformation("[Ordering] ---> Command successfully sent. Response: {@Response}", response);

                return response;
            }
            catch
            {
                return default!;
            }
        }

        protected virtual R CreateDefaultResult()
        {
            return default!;
        }
    }
}