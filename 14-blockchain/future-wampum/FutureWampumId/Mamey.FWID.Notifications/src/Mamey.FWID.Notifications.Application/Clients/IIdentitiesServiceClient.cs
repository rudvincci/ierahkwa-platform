using Mamey.FWID.Notifications.Application.DTO;

namespace Mamey.FWID.Notifications.Application.Clients;

/// <summary>
/// Service client for calling the Identities service.
/// </summary>
internal interface IIdentitiesServiceClient
{
    /// <summary>
    /// Gets an identity by its identifier.
    /// </summary>
    Task<IdentityDto?> GetIdentityAsync(Guid identityId, CancellationToken cancellationToken = default);
}







