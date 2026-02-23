using Mamey.FWID.Identities.Application.DTO;

namespace Mamey.FWID.Identities.Application.Clients;

/// <summary>
/// Service client for calling the AccessControls service.
/// </summary>
internal interface IAccessControlsServiceClient
{
    /// <summary>
    /// Checks if an identity has access to a zone.
    /// </summary>
    Task<bool> CheckZoneAccessAsync(Guid identityId, Guid zoneId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets access controls for an identity.
    /// </summary>
    Task<IReadOnlyList<AccessControlDto>> GetAccessControlsByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
}


