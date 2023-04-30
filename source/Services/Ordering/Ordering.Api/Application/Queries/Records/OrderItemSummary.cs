namespace Ordering.Api.Application.Queries.Records
{
    public record OrderItemSummary
    {
        public string? ProductName { get; init; }
        public int Units { get; init; }
        public decimal UnitPrice { get; init; }
        public string? PictureUrl { get; init; }
    }
}