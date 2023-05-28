namespace Ordering.Api.Application.Queries
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDetails>
    {
        private readonly ISqlConnectionFactory connectionFactory;

        public GetOrderQueryHandler(ISqlConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<OrderDetails> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            using (var connection = connectionFactory.GetDbConnection())
            {
                connection.Open();

                const string orderSQL =
                    @"SELECT TOP 1
                        o.Id, o.OrderDate AS Date, o.Description, (SELECT SUM(oi.Units*oi.UnitPrice) 
                            FROM ordering.orderitems oi WHERE oi.OrderId=@id) AS Total,
                        o.Address_Street as Street, o.Address_City as City, 
                        o.Address_State as State, o.Address_Country as Country,
                        o.Address_ZipCode as ZipCode, os.Name as Status, b.IdentityId as BuyerId,
                        oi.ProductName, oi.Units, oi.UnitPrice, oi.PictureUrl
                        FROM ordering.orders o
                        LEFT JOIN ordering.orderitems oi ON o.Id = oi.OrderId
                        LEFT JOIN ordering.buyers b ON o.BuyerId = b.Id
                        LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
                        WHERE o.Id=@id";

                var order = await connection.QueryFirstOrDefaultAsync<OrderDetails>(orderSQL, new { id = request.OrderId });

                connection.Close();

                if (order is null)
                {
                    throw new KeyNotFoundException();
                }

                return order;
            }
        }
    }
}