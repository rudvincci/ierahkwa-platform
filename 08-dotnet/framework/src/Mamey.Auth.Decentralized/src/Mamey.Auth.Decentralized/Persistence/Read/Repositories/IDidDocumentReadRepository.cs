using Mamey.Auth.Decentralized.Persistence.Read.Models;

namespace Mamey.Auth.Decentralized.Persistence.Read.Repositories;

/// <summary>
/// Repository interface for DID Document read operations
/// </summary>
public interface IDidDocumentReadRepository
{
    /// <summary>
    /// Gets a DID Document by its DID
    /// </summary>
    /// <param name="did">The DID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The DID Document read model, or null if not found</returns>
    Task<DidDocumentReadModel?> GetByDidAsync(string did, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a DID Document by its ID
    /// </summary>
    /// <param name="id">The document ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The DID Document read model, or null if not found</returns>
    Task<DidDocumentReadModel?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets DID Documents by method
    /// </summary>
    /// <param name="method">The DID method</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of DID Document read models</returns>
    Task<IReadOnlyList<DidDocumentReadModel>> GetByMethodAsync(string method, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets DID Documents by controller
    /// </summary>
    /// <param name="controller">The controller</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of DID Document read models</returns>
    Task<IReadOnlyList<DidDocumentReadModel>> GetByControllerAsync(string controller, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets DID Documents by tag
    /// </summary>
    /// <param name="tag">The tag</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of DID Document read models</returns>
    Task<IReadOnlyList<DidDocumentReadModel>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active DID Documents
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active DID Document read models</returns>
    Task<IReadOnlyList<DidDocumentReadModel>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches DID Documents by query
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching DID Document read models</returns>
    Task<IReadOnlyList<DidDocumentReadModel>> SearchAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated DID Documents
    /// </summary>
    /// <param name="page">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated result of DID Document read models</returns>
    Task<PaginatedResult<DidDocumentReadModel>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets DID Documents created after a specific date
    /// </summary>
    /// <param name="date">The date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of DID Document read models</returns>
    Task<IReadOnlyList<DidDocumentReadModel>> GetCreatedAfterAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets DID Documents updated after a specific date
    /// </summary>
    /// <param name="date">The date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of DID Document read models</returns>
    Task<IReadOnlyList<DidDocumentReadModel>> GetUpdatedAfterAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts DID Documents by method
    /// </summary>
    /// <param name="method">The DID method</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count</returns>
    Task<long> CountByMethodAsync(string method, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all active DID Documents
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count</returns>
    Task<long> CountActiveAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Paginated result for DID Documents
/// </summary>
/// <typeparam name="T">The type of items</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// The items in the current page
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// The total count of items
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// The current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
