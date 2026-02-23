namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Models;

/// <summary>
/// Parameters for querying a paged user list.
/// </summary>
public class UserQueryParameters
{
    /// <summary>Zero‑based page index.</summary>
    public int Page { get; set; }

    /// <summary>Number of items per page.</summary>
    public int PageSize { get; set; }

    /// <summary>Name of the property to sort by.</summary>
    public string? SortBy { get; set; }

    /// <summary>Whether to sort descending.</summary>
    public bool SortDescending { get; set; }

    /// <summary>Filter text (search term).</summary>
    public string? Filter { get; set; }
}