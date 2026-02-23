using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Contracts.DTOs;

namespace Mamey.FWID.Identities.Contracts.Queries;

/// <summary>
/// Query to verify an identity's biometric data.
/// Note: Queries should NOT have [Contract] attribute.
/// </summary>
public record VerifyIdentity(Guid IdentityId, BiometricDataDto ProvidedBiometric) : IQuery<BiometricVerificationResultDto>;

