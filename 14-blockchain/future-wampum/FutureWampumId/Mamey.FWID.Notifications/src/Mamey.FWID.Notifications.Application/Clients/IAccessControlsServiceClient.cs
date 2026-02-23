using Mamey.FWID.Notifications.Application.DTO;

namespace Mamey.FWID.Notifications.Application.Clients;

/// <summary>
/// Service client for calling the AccessControls service.
/// </summary>
internal interface IAccessControlsServiceClient
{
    /// <summary>
    /// Gets an access control by its identifier.
    /// </summary>
    Task<AccessControlDto?> GetAccessControlAsync(Guid accessControlId, CancellationToken cancellationToken = default);
}







