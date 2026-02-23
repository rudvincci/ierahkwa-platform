using Mamey.FWID.Notifications.Application.DTO;

namespace Mamey.FWID.Notifications.Application.Clients;

/// <summary>
/// Service client for calling the ZKPs service.
/// </summary>
internal interface IZKPsServiceClient
{
    /// <summary>
    /// Gets a ZKP proof by its identifier.
    /// </summary>
    Task<ZKPProofDto?> GetZKPProofAsync(Guid proofId, CancellationToken cancellationToken = default);
}







