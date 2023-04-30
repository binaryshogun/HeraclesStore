namespace Ordering.Api.Application.Queries.Records
{
    public record OrderSummary
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public string? Status { get; init; }
        public decimal Total { get; init; }
    }
}