using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a DID Resolution aggregate root for blockchain-based DID resolution.
/// </summary>
internal class DIDResolution : AggregateRoot<DIDResolutionId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private DIDResolution()
    {
        ResolutionAttempts = new List<ResolutionAttempt>();
        BlockchainQueries = new List<BlockchainQuery>();
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the DIDResolution aggregate root.
    /// </summary>
    /// <param name="id">The DID resolution identifier.</param>
    /// <param name="did">The DID to resolve.</param>
    /// <param name="requestedBy">The entity requesting the resolution.</param>
    /// <param name="resolutionType">The type of resolution required.</param>
    public DIDResolution(
        DIDResolutionId id,
        string did,
        string requestedBy,
        ResolutionType resolutionType)
        : base(id)
    {
        DID = did ?? throw new ArgumentNullException(nameof(did));
        RequestedBy = requestedBy ?? throw new ArgumentNullException(nameof(requestedBy));
        ResolutionType = resolutionType;
        Status = ResolutionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ResolutionAttempts = new List<ResolutionAttempt>();
        BlockchainQueries = new List<BlockchainQuery>();
        Metadata = new Dictionary<string, object>();
        Version = 1;

        AddEvent(new DIDResolutionRequested(Id, DID, RequestedBy, ResolutionType, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The DID being resolved.
    /// </summary>
    public string DID { get; private set; }

    /// <summary>
    /// The entity that requested the resolution.
    /// </summary>
    public string RequestedBy { get; private set; }

    /// <summary>
    /// The type of resolution required.
    /// </summary>
    public ResolutionType ResolutionType { get; private set; }

    /// <summary>
    /// The current status of the resolution.
    /// </summary>
    public ResolutionStatus Status { get; private set; }

    /// <summary>
    /// When the resolution was requested.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the resolution was completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// The resolved DID document.
    /// </summary>
    public string? ResolvedDocument { get; private set; }

    /// <summary>
    /// The blockchain transaction hash used for resolution.
    /// </summary>
    public string? BlockchainTxHash { get; private set; }

    /// <summary>
    /// The block number where the DID was found.
    /// </summary>
    public long? BlockNumber { get; private set; }

    /// <summary>
    /// The confidence score of the resolution.
    /// </summary>
    public int? ConfidenceScore { get; private set; }

    /// <summary>
    /// The resolution attempts made.
    /// </summary>
    public List<ResolutionAttempt> ResolutionAttempts { get; private set; }

    /// <summary>
    /// The blockchain queries performed.
    /// </summary>
    public List<BlockchainQuery> BlockchainQueries { get; private set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Starts the resolution process.
    /// </summary>
    public void StartResolution()
    {
        if (Status != ResolutionStatus.Pending)
            throw new InvalidOperationException("Resolution can only be started from pending status");

        Status = ResolutionStatus.Resolving;
        IncrementVersion();

        AddEvent(new DIDResolutionStarted(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Records a blockchain query attempt.
    /// </summary>
    /// <param name="queryType">The type of query performed.</param>
    /// <param name="endpoint">The blockchain endpoint queried.</param>
    /// <param name="responseTime">The response time in milliseconds.</param>
    /// <param name="success">Whether the query was successful.</param>
    public void RecordBlockchainQuery(
        string queryType,
        string endpoint,
        long responseTime,
        bool success)
    {
        var query = new BlockchainQuery(
            queryType,
            endpoint,
            responseTime,
            success,
            DateTime.UtcNow);

        BlockchainQueries.Add(query);
        IncrementVersion();
    }

    /// <summary>
    /// Records a resolution attempt.
    /// </summary>
    /// <param name="method">The resolution method used.</param>
    /// <param name="result">The result of the attempt.</param>
    /// <param name="errorMessage">Any error message.</param>
    public void RecordResolutionAttempt(
        string method,
        ResolutionAttemptResult result,
        string? errorMessage = null)
    {
        var attempt = new ResolutionAttempt(
            method,
            result,
            errorMessage,
            DateTime.UtcNow);

        ResolutionAttempts.Add(attempt);
        IncrementVersion();

        AddEvent(new ResolutionAttemptRecorded(Id, method, result, DateTime.UtcNow));
    }

    /// <summary>
    /// Completes the resolution successfully.
    /// </summary>
    /// <param name="resolvedDocument">The resolved DID document.</param>
    /// <param name="blockchainTxHash">The blockchain transaction hash.</param>
    /// <param name="blockNumber">The block number.</param>
    /// <param name="confidenceScore">The confidence score.</param>
    public void CompleteResolution(
        string resolvedDocument,
        string blockchainTxHash,
        long blockNumber,
        int confidenceScore)
    {
        if (Status != ResolutionStatus.Resolving)
            throw new InvalidOperationException("Resolution must be in progress to complete");

        ResolvedDocument = resolvedDocument;
        BlockchainTxHash = blockchainTxHash;
        BlockNumber = blockNumber;
        ConfidenceScore = confidenceScore;
        Status = ResolutionStatus.Resolved;
        CompletedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new DIDResolutionCompleted(Id, blockchainTxHash, blockNumber, confidenceScore, CompletedAt.Value));
    }

    /// <summary>
    /// Fails the resolution.
    /// </summary>
    /// <param name="reason">The reason for failure.</param>
    public void FailResolution(string reason)
    {
        if (Status == ResolutionStatus.Failed)
            return;

        Status = ResolutionStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new DIDResolutionFailed(Id, reason, CompletedAt.Value));
    }

    /// <summary>
    /// Updates the resolution with cached data.
    /// </summary>
    /// <param name="cachedDocument">The cached DID document.</param>
    /// <param name="cacheTimestamp">When the cache was created.</param>
    public void UpdateFromCache(string cachedDocument, DateTime cacheTimestamp)
    {
        ResolvedDocument = cachedDocument;
        Status = ResolutionStatus.Resolved;
        CompletedAt = DateTime.UtcNow;
        ConfidenceScore = 95; // High confidence for cached data
        IncrementVersion();

        AddEvent(new DIDResolutionFromCache(Id, cacheTimestamp, 1));
    }

    /// <summary>
    /// Checks if the resolution is still valid.
    /// </summary>
    /// <returns>True if the resolution is valid and not expired.</returns>
    public bool IsValid()
    {
        return Status == ResolutionStatus.Resolved &&
               CompletedAt.HasValue &&
               (DateTime.UtcNow - CompletedAt.Value) < TimeSpan.FromHours(24); // 24-hour validity
    }

    /// <summary>
    /// Gets the resolution statistics.
    /// </summary>
    /// <returns>Statistics about the resolution process.</returns>
    public ResolutionStatistics GetStatistics()
    {
        var totalAttempts = ResolutionAttempts.Count;
        var successfulAttempts = ResolutionAttempts.Count(a => a.Result == ResolutionAttemptResult.Success);
        var averageResponseTime = BlockchainQueries.Any() ?
            BlockchainQueries.Average(q => q.ResponseTimeMs) : 0;

        return new ResolutionStatistics(
            totalAttempts,
            successfulAttempts,
            averageResponseTime,
            BlockchainQueries.Count);
    }

    #endregion
}

/// <summary>
/// Represents the type of resolution.
/// </summary>
internal enum ResolutionType
{
    FullDocument,
    VerificationMethods,
    ServiceEndpoints,
    MetadataOnly
}

/// <summary>
/// Represents the status of a DID resolution.
/// </summary>
internal enum ResolutionStatus
{
    Pending,
    Resolving,
    Resolved,
    Failed
}

/// <summary>
/// Represents the result of a resolution attempt.
/// </summary>
internal enum ResolutionAttemptResult
{
    Success,
    NetworkError,
    NotFound,
    InvalidFormat,
    Timeout,
    AuthenticationFailed
}

/// <summary>
/// Represents a resolution attempt.
/// </summary>
internal class ResolutionAttempt
{
    public string Method { get; set; }
    public ResolutionAttemptResult Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime AttemptedAt { get; set; }

    public ResolutionAttempt(
        string method,
        ResolutionAttemptResult result,
        string? errorMessage,
        DateTime attemptedAt)
    {
        Method = method;
        Result = result;
        ErrorMessage = errorMessage;
        AttemptedAt = attemptedAt;
    }
}

/// <summary>
/// Represents a blockchain query.
/// </summary>
internal class BlockchainQuery
{
    public string QueryType { get; set; }
    public string Endpoint { get; set; }
    public long ResponseTimeMs { get; set; }
    public bool Success { get; set; }
    public DateTime QueriedAt { get; set; }

    public BlockchainQuery(
        string queryType,
        string endpoint,
        long responseTimeMs,
        bool success,
        DateTime queriedAt)
    {
        QueryType = queryType;
        Endpoint = endpoint;
        ResponseTimeMs = responseTimeMs;
        Success = success;
        QueriedAt = queriedAt;
    }
}

/// <summary>
/// Represents resolution statistics.
/// </summary>
internal class ResolutionStatistics
{
    public int TotalAttempts { get; set; }
    public int SuccessfulAttempts { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public int TotalQueries { get; set; }

    public ResolutionStatistics(
        int totalAttempts,
        int successfulAttempts,
        double averageResponseTimeMs,
        int totalQueries)
    {
        TotalAttempts = totalAttempts;
        SuccessfulAttempts = successfulAttempts;
        AverageResponseTimeMs = averageResponseTimeMs;
        TotalQueries = totalQueries;
    }
}
