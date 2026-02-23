using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IFutureWampumIdClient
{
    Task<List<IdentityVerificationInfo>> GetIdentityVerificationsAsync();
    Task<List<DigitalCredentialInfo>> GetDigitalCredentialsAsync();
    Task<List<IdentityWalletInfo>> GetIdentityWalletsAsync();
    Task<List<AttestationInfo>> GetAttestationsAsync();
    Task<List<RecoveryInfo>> GetRecoveriesAsync();
    Task<List<DIDDocumentInfo>> GetDIDDocumentsAsync();
    Task<List<DIDHistoryEntryInfo>> GetDIDHistoryAsync(string did);
}

