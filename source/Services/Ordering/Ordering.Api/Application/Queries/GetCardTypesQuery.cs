namespace Ordering.Api.Application.Queries
{
    public class GetCardTypesQuery : IRequest<IEnumerable<CardTypeSummary>>
    {
        public GetCardTypesQuery() { }
    }
}