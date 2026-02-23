namespace Mamey.FWID.Identities.Application.Clients;

/// <summary>
/// Client for integrating with FutureWampumLedger.Transaction service
/// Logs identity-related transactions to the immutable ledger
/// </summary>
internal interface ILedgerTransactionClient
{
    /// <summary>
    /// Logs a transaction to the FutureWampumLedger.Transaction service
    /// </summary>
    /// <param name="request">Transaction log request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transaction was logged successfully, false otherwise</returns>
    Task<bool> LogTransactionAsync(TransactionLogRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Request model for logging transactions to the ledger
/// </summary>
public class TransactionLogRequest
{
    public string TransactionType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
}

