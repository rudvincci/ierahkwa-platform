using Mamey.Auth.Decentralized.Persistence.Read.Models;

namespace Mamey.Auth.Decentralized.Persistence.Read.Repositories;

/// <summary>
/// Repository interface for Service Endpoint read operations
/// </summary>
public interface IServiceEndpointReadRepository
{
    /// <summary>
    /// Gets a service endpoint by its ID
    /// </summary>
    /// <param name="serviceEndpointId">The service endpoint ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The service endpoint read model, or null if not found</returns>
    Task<ServiceEndpointReadModel?> GetByIdAsync(string serviceEndpointId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets service endpoints by DID
    /// </summary>
    /// <param name="did">The DID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of service endpoint read models</returns>
    Task<IReadOnlyList<ServiceEndpointReadModel>> GetByDidAsync(string did, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets service endpoints by type
    /// </summary>
    /// <param name="type">The service endpoint type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of service endpoint read models</returns>
    Task<IReadOnlyList<ServiceEndpointReadModel>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets service endpoints by URL
    /// </summary>
    /// <param name="url">The service endpoint URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of service endpoint read models</returns>
    Task<IReadOnlyList<ServiceEndpointReadModel>> GetByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets service endpoints by tag
    /// </summary>
    /// <param name="tag">The tag</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of service endpoint read models</returns>
    Task<IReadOnlyList<ServiceEndpointReadModel>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets service endpoints by priority
    /// </summary>
    /// <param name="priority">The priority</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of service endpoint read models</returns>
    Task<IReadOnlyList<ServiceEndpointReadModel>> GetByPriorityAsync(int priority, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active service endpoints
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active service endpoint read models</returns>
    Task<IReadOnlyList<ServiceEndpointReadModel>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches service endpoints by query
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching service endpoint read models</returns>
    Task<IReadOnlyList<ServiceEndpointReadModel>> SearchAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated service endpoints
    /// </summary>
    /// <param name="page">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated result of service endpoint read models</returns>
    Task<PaginatedResult<ServiceEndpointReadModel>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts service endpoints by type
    /// </summary>
    /// <param name="type">The service endpoint type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count</returns>
    Task<long> CountByTypeAsync(string type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all active service endpoints
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count</returns>
    Task<long> CountActiveAsync(CancellationToken cancellationToken = default);
}
