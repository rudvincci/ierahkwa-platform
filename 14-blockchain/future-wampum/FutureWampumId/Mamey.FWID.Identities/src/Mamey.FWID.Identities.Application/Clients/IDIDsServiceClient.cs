using Mamey.FWID.Identities.Application.DTO;

namespace Mamey.FWID.Identities.Application.Clients;

/// <summary>
/// Service client for calling the DIDs service.
/// </summary>
internal interface IDIDsServiceClient
{
    /// <summary>
    /// Gets a DID by its identifier.
    /// </summary>
    Task<DIDDto?> GetDIDAsync(Guid didId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a DID by identity identifier.
    /// </summary>
    Task<DIDDto?> GetDIDByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
}


