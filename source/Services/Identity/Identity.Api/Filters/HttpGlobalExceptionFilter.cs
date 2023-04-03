namespace Identity.Api.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment environment;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment environment, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.environment = environment;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

            if (context.Exception is IdentityException)
            {
                var problemDetails = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional information"
                };

                problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

                context.Result = new BadRequestObjectResult(problemDetails);
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                const string serverError = "Internal server error. Try again";

                if (environment.IsDevelopment())
                {
                    context.Result = new ObjectResult(
                        new
                        {
                            Message = serverError,
                            DeveloperMessage = context.Exception.Message
                        });
                }
                else
                {
                    context.Result = new ObjectResult(new { Message = serverError });
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            context.ExceptionHandled = true;
        }
    }
}