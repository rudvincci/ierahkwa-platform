namespace Mamey.Government.UI.Models;

/// <summary>
/// Client-side paged result model matching the API response.
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public long TotalResults { get; set; }
    
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    
    public static PagedResult<T> Empty => new()
    {
        Items = Array.Empty<T>(),
        Page = 1,
        PageSize = 20,
        TotalPages = 0,
        TotalResults = 0
    };
}
