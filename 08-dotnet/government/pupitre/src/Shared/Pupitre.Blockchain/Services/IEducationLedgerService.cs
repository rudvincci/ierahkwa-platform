using Pupitre.Blockchain.Models;

namespace Pupitre.Blockchain.Services;

/// <summary>
/// Abstraction that encapsulates all blockchain/Government interactions required by Pupitre services.
/// </summary>
public interface IEducationLedgerService
{
    /// <summary>
    /// Issues or updates a learning credential on the MameyNode ledger.
    /// </summary>
    Task<EducationLedgerReceipt> PublishCredentialAsync(EducationLedgerPayload payload, CancellationToken cancellationToken = default);
}
