using System.Collections.Generic;
using System.Linq;

namespace Mamey.MicroMonolith.Abstractions.Queries;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int CurrentPage { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public PagedResult(IEnumerable<T> items, int currentPage, int pageSize, int totalCount)
    {
        Items = items.ToList();
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static PagedResult<T> Empty(int page = 1, int pageSize = 20)
        => new(Enumerable.Empty<T>(), page, pageSize, 0);

    public PagedResult<TResult> Map<TResult>(System.Func<T, TResult> mapper)
        => new(Items.Select(mapper), CurrentPage, PageSize, TotalCount);
}
