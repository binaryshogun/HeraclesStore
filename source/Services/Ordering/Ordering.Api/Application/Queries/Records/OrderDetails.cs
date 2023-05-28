namespace Ordering.Api.Application.Queries.Records
{
    public record OrderDetails
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public string? Status { get; init; }
        public string? Description { get; init; }
        public string? Street { get; init; }
        public string? City { get; init; }
        public string? State { get; init; }
        public string? Country { get; init; }
        public string? ZipCode { get; init; }
        public decimal Total { get; init; }

        public Guid BuyerId { get; init; }

        public List<OrderItemSummary>? OrderItems { get; set; }
    }
}