namespace Bff.Web.Network
{
    public static class NetworkConfigurator
    {
        public static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration configuration)
        {
            var httpPort = configuration.GetSection("NETWORK").GetValue<int>("HTTP_PORT", 80);
            var grpcPort = configuration.GetSection("NETWORK").GetValue<int>("GRPC_PORT", 81);

            return (httpPort, grpcPort);
        }
    }
}