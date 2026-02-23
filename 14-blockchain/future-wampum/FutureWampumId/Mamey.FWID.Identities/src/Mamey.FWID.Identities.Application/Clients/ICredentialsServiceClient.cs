using Mamey.FWID.Identities.Application.DTO;

namespace Mamey.FWID.Identities.Application.Clients;

/// <summary>
/// Service client for calling the Credentials service.
/// </summary>
internal interface ICredentialsServiceClient
{
    /// <summary>
    /// Gets a credential by its identifier.
    /// </summary>
    Task<CredentialDto?> GetCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets credentials by identity identifier.
    /// </summary>
    Task<IReadOnlyList<CredentialDto>> GetCredentialsByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
}


