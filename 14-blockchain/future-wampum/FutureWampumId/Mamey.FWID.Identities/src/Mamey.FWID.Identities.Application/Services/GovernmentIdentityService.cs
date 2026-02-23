using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service implementation for Government Identity blockchain operations.
/// Registers sovereign identities on the MameyNode blockchain via Mamey.Blockchain.Government.
/// 
/// TDD Reference: Line 1594-1703 (Feature Domain 1: Biometric Identity)
/// BDD Reference: Lines 86-112 (I.1-I.3 Executive Summary - Sovereign Identity)
/// </summary>
internal sealed class GovernmentIdentityService : IGovernmentIdentityService
{
    private readonly IGovernmentIdentityClient _governmentClient;
    private readonly ILogger<GovernmentIdentityService> _logger;

    public GovernmentIdentityService(
        IGovernmentIdentityClient governmentClient,
        ILogger<GovernmentIdentityService> logger)
    {
        _governmentClient = governmentClient ?? throw new ArgumentNullException(nameof(governmentClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GovernmentIdentityResult> RegisterIdentityAsync(
        Identity identity,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Registering sovereign identity on blockchain: IdentityId={IdentityId}, Name={Name}",
            identity.Id.Value, identity.Name?.FullName);

        try
        {
            var metadata = new Dictionary<string, string>
            {
                ["zone"] = identity.Zone ?? "Unknown",
                ["createdAt"] = identity.CreatedAt.ToString("O"),
                ["correlationId"] = correlationId ?? Guid.NewGuid().ToString()
            };

            if (identity.ClanRegistrarId.HasValue)
            {
                metadata["clanRegistrarId"] = identity.ClanRegistrarId.Value.ToString();
            }

            var request = new CreateIdentityRequest(
                CitizenId: identity.Id.Value.ToString(),
                FirstName: identity.Name?.FirstName ?? "",
                LastName: identity.Name?.LastName ?? "",
                DateOfBirth: identity.PersonalDetails?.DateOfBirth?.ToString("yyyy-MM-dd") ?? "",
                Nationality: identity.PersonalDetails?.ClanAffiliation ?? "",
                Metadata: metadata);

            var result = await _governmentClient.CreateIdentityAsync(request, cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Successfully registered sovereign identity on blockchain: IdentityId={IdentityId}, BlockchainId={BlockchainId}",
                    identity.Id.Value, result.IdentityId);

                return new GovernmentIdentityResult(
                    Success: true,
                    BlockchainIdentityId: result.IdentityId,
                    BlockchainAccount: result.BlockchainAccount,
                    ErrorMessage: null);
            }

            _logger.LogWarning(
                "Failed to register sovereign identity on blockchain: IdentityId={IdentityId}, Error={Error}",
                identity.Id.Value, result.ErrorMessage);

            return new GovernmentIdentityResult(
                Success: false,
                BlockchainIdentityId: null,
                BlockchainAccount: null,
                ErrorMessage: result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error registering sovereign identity on blockchain: IdentityId={IdentityId}",
                identity.Id.Value);

            return new GovernmentIdentityResult(
                Success: false,
                BlockchainIdentityId: null,
                BlockchainAccount: null,
                ErrorMessage: ex.Message);
        }
    }

    public async Task<GovernmentIdentityResult> UpdateIdentityAsync(
        Identity identity,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Updating sovereign identity on blockchain: IdentityId={IdentityId}",
            identity.Id.Value);

        try
        {
            var updates = new Dictionary<string, string>
            {
                ["firstName"] = identity.Name?.FirstName ?? "",
                ["lastName"] = identity.Name?.LastName ?? "",
                ["updatedAt"] = DateTime.UtcNow.ToString("O")
            };

            if (identity.PersonalDetails?.ClanAffiliation != null)
            {
                updates["nationality"] = identity.PersonalDetails.ClanAffiliation;
            }

            var result = await _governmentClient.UpdateIdentityAsync(
                identity.Id.Value.ToString(),
                updates,
                cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Successfully updated sovereign identity on blockchain: IdentityId={IdentityId}",
                    identity.Id.Value);

                return new GovernmentIdentityResult(
                    Success: true,
                    BlockchainIdentityId: identity.Id.Value.ToString(),
                    BlockchainAccount: null,
                    ErrorMessage: null);
            }

            _logger.LogWarning(
                "Failed to update sovereign identity on blockchain: IdentityId={IdentityId}, Error={Error}",
                identity.Id.Value, result.ErrorMessage);

            return new GovernmentIdentityResult(
                Success: false,
                BlockchainIdentityId: null,
                BlockchainAccount: null,
                ErrorMessage: result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error updating sovereign identity on blockchain: IdentityId={IdentityId}",
                identity.Id.Value);

            return new GovernmentIdentityResult(
                Success: false,
                BlockchainIdentityId: null,
                BlockchainAccount: null,
                ErrorMessage: ex.Message);
        }
    }

    public async Task<GovernmentIdentityVerificationResult> VerifyIdentityAsync(
        Identity identity,
        string verificationType = "full",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Verifying sovereign identity against blockchain: IdentityId={IdentityId}, Type={Type}",
            identity.Id.Value, verificationType);

        try
        {
            var result = await _governmentClient.VerifyIdentityAsync(
                identity.Id.Value.ToString(),
                verificationType,
                cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Identity verification completed: IdentityId={IdentityId}, Verified={Verified}",
                    identity.Id.Value, result.Verified);
            }
            else
            {
                _logger.LogWarning(
                    "Identity verification failed: IdentityId={IdentityId}, Error={Error}",
                    identity.Id.Value, result.ErrorMessage);
            }

            return new GovernmentIdentityVerificationResult(
                Verified: result.Verified,
                VerificationResult: result.VerificationResult,
                Success: result.Success,
                ErrorMessage: result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error verifying identity against blockchain: IdentityId={IdentityId}",
                identity.Id.Value);

            return new GovernmentIdentityVerificationResult(
                Verified: false,
                VerificationResult: null,
                Success: false,
                ErrorMessage: ex.Message);
        }
    }

    public async Task<GovernmentBlockchainIdentity?> GetIdentityFromBlockchainAsync(
        string identityId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting identity from blockchain: IdentityId={IdentityId}", identityId);

        try
        {
            var result = await _governmentClient.GetIdentityAsync(identityId, cancellationToken);

            if (result == null || !result.Success)
            {
                _logger.LogDebug("Identity not found on blockchain: IdentityId={IdentityId}", identityId);
                return null;
            }

            return new GovernmentBlockchainIdentity(
                IdentityId: result.IdentityId,
                CitizenId: result.CitizenId,
                FirstName: result.FirstName,
                LastName: result.LastName,
                DateOfBirth: result.DateOfBirth,
                Nationality: result.Nationality,
                Status: result.Status,
                RegistrationDate: null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting identity from blockchain: IdentityId={IdentityId}", identityId);
            return null;
        }
    }

    public async Task<GovernmentIdentityResult> RecordClanRegistrarAuthorizationAsync(
        string identityId,
        Guid clanRegistrarId,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Recording clan registrar authorization on blockchain: IdentityId={IdentityId}, ClanRegistrar={ClanRegistrar}",
            identityId, clanRegistrarId);

        try
        {
            var updates = new Dictionary<string, string>
            {
                ["clanRegistrarId"] = clanRegistrarId.ToString(),
                ["clanRegistrarAuthorizedAt"] = DateTime.UtcNow.ToString("O"),
                ["correlationId"] = correlationId ?? Guid.NewGuid().ToString()
            };

            var result = await _governmentClient.UpdateIdentityAsync(
                identityId,
                updates,
                cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Successfully recorded clan registrar authorization on blockchain: IdentityId={IdentityId}",
                    identityId);

                return new GovernmentIdentityResult(
                    Success: true,
                    BlockchainIdentityId: identityId,
                    BlockchainAccount: null,
                    ErrorMessage: null);
            }

            _logger.LogWarning(
                "Failed to record clan registrar authorization: IdentityId={IdentityId}, Error={Error}",
                identityId, result.ErrorMessage);

            return new GovernmentIdentityResult(
                Success: false,
                BlockchainIdentityId: null,
                BlockchainAccount: null,
                ErrorMessage: result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error recording clan registrar authorization: IdentityId={IdentityId}",
                identityId);

            return new GovernmentIdentityResult(
                Success: false,
                BlockchainIdentityId: null,
                BlockchainAccount: null,
                ErrorMessage: ex.Message);
        }
    }
}
