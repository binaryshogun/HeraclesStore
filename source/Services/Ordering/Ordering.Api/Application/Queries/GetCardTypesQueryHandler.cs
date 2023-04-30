namespace Ordering.Api.Application.Queries
{
    public class GetCardTypesQueryHandler : IRequestHandler<GetCardTypesQuery, IEnumerable<CardTypeSummary>>
    {
        private readonly ISqlConnectionFactory connectionFactory;

        public GetCardTypesQueryHandler(ISqlConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<CardTypeSummary>> Handle(GetCardTypesQuery request, CancellationToken cancellationToken)
        {
            using (var connection = connectionFactory.GetDbConnection())
            {
                connection.Open();

                var cardtypes = await connection.QueryAsync<CardTypeSummary>(@"SELECT * FROM ordering.cardtypes");

                connection.Close();

                return cardtypes;
            }
        }
    }
}