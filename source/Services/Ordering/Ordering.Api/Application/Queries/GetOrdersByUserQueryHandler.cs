namespace Ordering.Api.Application.Queries
{
    public class GetOrdersByUserQueryHandler
        : IRequestHandler<GetOrdersByUserQuery, IEnumerable<OrderSummary>>
    {
        private readonly ISqlConnectionFactory connectionFactory;

        public GetOrdersByUserQueryHandler(ISqlConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<OrderSummary>> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
        {
            using (var connection = connectionFactory.GetDbConnection())
            {
                connection.Open();

                const string ordersByUserSQL =
                    @"SELECT o.Id, o.OrderDate AS Date, os.Name as Status, SUM(oi.Units*oi.UnitPrice) as Total
                        FROM ordering.orders o
                        LEFT JOIN ordering.orderitems oi ON o.Id = oi.OrderId
                        LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
                        LEFT JOIN ordering.buyers b on o.BuyerId = b.Id
                        WHERE b.IdentityId = @userId
                        GROUP BY o.Id, o.OrderDate, os.Name";

                var orders = await connection.QueryAsync<OrderSummary>(ordersByUserSQL, new { userId = request.UserId });

                connection.Close();

                return orders;
            }
        }
    }
}