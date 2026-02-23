using Mamey.Auth.Decentralized.Persistence.Read.Models;

namespace Mamey.Auth.Decentralized.Persistence.Read.Repositories;

/// <summary>
/// Repository interface for Verification Method read operations
/// </summary>
public interface IVerificationMethodReadRepository
{
    /// <summary>
    /// Gets a verification method by its ID
    /// </summary>
    /// <param name="verificationMethodId">The verification method ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The verification method read model, or null if not found</returns>
    Task<VerificationMethodReadModel?> GetByIdAsync(string verificationMethodId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets verification methods by DID
    /// </summary>
    /// <param name="did">The DID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of verification method read models</returns>
    Task<IReadOnlyList<VerificationMethodReadModel>> GetByDidAsync(string did, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets verification methods by type
    /// </summary>
    /// <param name="type">The verification method type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of verification method read models</returns>
    Task<IReadOnlyList<VerificationMethodReadModel>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets verification methods by algorithm
    /// </summary>
    /// <param name="algorithm">The algorithm</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of verification method read models</returns>
    Task<IReadOnlyList<VerificationMethodReadModel>> GetByAlgorithmAsync(string algorithm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets verification methods by curve
    /// </summary>
    /// <param name="curve">The curve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of verification method read models</returns>
    Task<IReadOnlyList<VerificationMethodReadModel>> GetByCurveAsync(string curve, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets verification methods by controller
    /// </summary>
    /// <param name="controller">The controller</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of verification method read models</returns>
    Task<IReadOnlyList<VerificationMethodReadModel>> GetByControllerAsync(string controller, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets verification methods by tag
    /// </summary>
    /// <param name="tag">The tag</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of verification method read models</returns>
    Task<IReadOnlyList<VerificationMethodReadModel>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active verification methods
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active verification method read models</returns>
    Task<IReadOnlyList<VerificationMethodReadModel>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches verification methods by query
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching verification method read models</returns>
    Task<IReadOnlyList<VerificationMethodReadModel>> SearchAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated verification methods
    /// </summary>
    /// <param name="page">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated result of verification method read models</returns>
    Task<PaginatedResult<VerificationMethodReadModel>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts verification methods by algorithm
    /// </summary>
    /// <param name="algorithm">The algorithm</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count</returns>
    Task<long> CountByAlgorithmAsync(string algorithm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all active verification methods
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count</returns>
    Task<long> CountActiveAsync(CancellationToken cancellationToken = default);
}
