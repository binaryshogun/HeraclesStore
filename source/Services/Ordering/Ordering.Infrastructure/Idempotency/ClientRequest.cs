namespace Ordering.Infrastructure.Idempotency
{
    /// <summary>
    /// Represents client request.
    /// </summary>
    public class ClientRequest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime Time { get; set; }
    }
}