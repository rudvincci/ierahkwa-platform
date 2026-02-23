namespace Pupitre.Contracts.DTOs;

/// <summary>
/// Represents a paginated result set.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed record PagedResult<T>
{
    /// <summary>
    /// The items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public long TotalCount { get; init; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Creates an empty paged result.
    /// </summary>
    public static PagedResult<T> Empty => new()
    {
        Items = Array.Empty<T>(),
        Page = 1,
        PageSize = 10,
        TotalCount = 0
    };

    /// <summary>
    /// Creates a paged result from items.
    /// </summary>
    public static PagedResult<T> Create(IReadOnlyList<T> items, int page, int pageSize, long totalCount)
        => new()
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
}
