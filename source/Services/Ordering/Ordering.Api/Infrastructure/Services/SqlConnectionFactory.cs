namespace Ordering.Api.Infrastructure.Services
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            this.connectionString = !string.IsNullOrEmpty(connectionString) ?
                connectionString :
                throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection GetDbConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}