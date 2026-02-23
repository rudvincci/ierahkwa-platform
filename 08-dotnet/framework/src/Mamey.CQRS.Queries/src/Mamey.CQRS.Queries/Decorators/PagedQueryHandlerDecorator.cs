using Mamey.Types;
namespace Mamey.CQRS.Queries.Decorators;

[Decorator]
internal sealed class PagedQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _handler;

    public PagedQueryHandlerDecorator(IQueryHandler<TQuery, TResult> handler)
    {
        _handler = handler;
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        const int maxResults = 100;
        const int defaultResults = 10;
            
        if (query is IPagedQuery pagedQuery)
        {
            if (pagedQuery.Page <= 0)
            {
                pagedQuery.Page = 1;
            }

            if (pagedQuery.ResultsPerPage <= 0)
            {
                pagedQuery.ResultsPerPage = defaultResults;
            }

            if (pagedQuery.ResultsPerPage > maxResults)
            {
                pagedQuery.ResultsPerPage = maxResults;
            }
        }

        return await _handler.HandleAsync(query, cancellationToken);
    }
}