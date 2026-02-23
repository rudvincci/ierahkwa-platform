using Mamey.FWID.Notifications.Application.DTO;

namespace Mamey.FWID.Notifications.Application.Clients;

/// <summary>
/// Service client for calling the Credentials service.
/// </summary>
internal interface ICredentialsServiceClient
{
    /// <summary>
    /// Gets a credential by its identifier.
    /// </summary>
    Task<CredentialDto?> GetCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default);
}







