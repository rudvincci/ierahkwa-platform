using Mamey.FWID.Notifications.Application.DTO;

namespace Mamey.FWID.Notifications.Application.Clients;

/// <summary>
/// Service client for calling the DIDs service.
/// </summary>
internal interface IDIDsServiceClient
{
    /// <summary>
    /// Gets a DID by identity identifier.
    /// </summary>
    Task<DIDDto?> GetDIDByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
}







