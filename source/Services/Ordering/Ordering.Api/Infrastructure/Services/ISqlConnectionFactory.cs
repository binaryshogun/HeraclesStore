namespace Ordering.Api.Infrastructure.Services
{
    public interface ISqlConnectionFactory
    {
        public IDbConnection GetDbConnection();
    }
}