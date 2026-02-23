namespace Mamey.Authentik.Models;

/// <summary>
/// Represents a paginated result from Authentik API.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// Gets or sets the list of results.
    /// </summary>
    public List<T> Results { get; set; } = new();

    /// <summary>
    /// Gets or sets the pagination information.
    /// </summary>
    public Pagination Pagination { get; set; } = new();
}

/// <summary>
/// Pagination information.
/// </summary>
public class Pagination
{
    /// <summary>
    /// Gets or sets the total count of items.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the URL for the next page, if available.
    /// </summary>
    public string? Next { get; set; }

    /// <summary>
    /// Gets or sets the URL for the previous page, if available.
    /// </summary>
    public string? Previous { get; set; }

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNext => !string.IsNullOrWhiteSpace(Next);

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPrevious => !string.IsNullOrWhiteSpace(Previous);
}
