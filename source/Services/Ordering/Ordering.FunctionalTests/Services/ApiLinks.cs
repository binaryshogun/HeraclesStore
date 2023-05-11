namespace Ordering.FunctionalTests.Services
{
    public class ApiLinks
    {
        private static string api = "api/orders";

        public static string GetOrders() => api;
        public static string GetOrder(int orderId) => $"{api}/{orderId}";
        public static string GetCardTypes() => $"{api}/cardtypes";
        public static string ShipOrder() => $"{api}/ship";
        public static string CancelOrder() => $"{api}/cancel";
    }
}