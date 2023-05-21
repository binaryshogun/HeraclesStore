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
                        o.Id, o.OrderDate, o.Description, SUM(oi.Units*oi.UnitPrice) as Total, 
                        o.Address_Street as Street, o.Address_City as City, 
                        o.Address_State as State, o.Address_Country as Country,
                        o.Address_ZipCode as ZipCode, os.Name as Status, o.BuyerId
                        oi.ProductName, oi.Units, oi.UnitPrice, oi.PictureUrl
                        FROM ordering.orders o
                        LEFT JOIN ordering.orderitems oi ON o.Id = oi.OrderId
                        LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
                        WHERE o.Id=@id
                        GROUP BY o.ID, o.OrderDate, o.Description, 
                        o.Address_Street, o.Address_City, 
                        o.Address_State, o.Address_Country, 
                        o.Address_ZipCode, os.Name,
                        oi.ProductName, oi.Units, 
                        oi.UnitPrice, oi.PictureUrl";

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