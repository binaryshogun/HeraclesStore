using Ordering.FunctionalTests.Services;

namespace Ordering.FunctionalTests.Extensions
{
    internal static class WebApplicationFactoryExtensions
    {
        public static HttpClient CreateIdempotentClient(this OrderingWebApplicationFactory factory)
        {
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());

            return client;
        }
    }
}