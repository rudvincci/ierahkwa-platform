using Mamey.FWID.Identities.Application.DTO;

namespace Mamey.FWID.Identities.Application.Clients;

/// <summary>
/// Service client for calling the ZKPs service.
/// </summary>
internal interface IZKPsServiceClient
{
    /// <summary>
    /// Gets a ZKP proof by its identifier.
    /// </summary>
    Task<ZKPProofDto?> GetZKPProofAsync(Guid proofId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets ZKP proofs by identity identifier.
    /// </summary>
    Task<IReadOnlyList<ZKPProofDto>> GetZKPProofsByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
}


