using System.Text.Json.Serialization;

using System;
using System.Collections.Generic;

namespace Mamey.Biometrics.Services.Models;

/// <summary>
/// Paged result for template queries.
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Items in current page
    /// </summary>
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Total count of items
    /// </summary>
    [JsonPropertyName("total_count")]
    public long TotalCount { get; set; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    [JsonPropertyName("total_pages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    [JsonPropertyName("has_next_page")]
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    [JsonPropertyName("has_previous_page")]
    public bool HasPreviousPage => Page > 1;
}
